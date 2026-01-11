using Mapster;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class DropoutService : GenericService<DropoutRequest, DropoutResponse, Dropout>, IDropoutService
    {
        private readonly IDropoutRepository _dropoutRepository;
        private readonly IProgramRepository _programRepository;
        private readonly IProgramEnrollmentRepository _programEnrollmentRepository;
        private readonly IStudentSectionRepository _studentSectionRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DropoutService(
            IDropoutRepository dropoutRepository,
            IProgramRepository programRepository,
            IProgramEnrollmentRepository programEnrollmentRepository,
            IStudentSectionRepository studentSectionRepository,
            ISectionRepository sectionRepository,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor) : base(dropoutRepository)
        {
            _dropoutRepository = dropoutRepository;
            _programRepository = programRepository;
            _programEnrollmentRepository = programEnrollmentRepository;
            _studentSectionRepository = studentSectionRepository;
            _sectionRepository = sectionRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var dropouts = await _dropoutRepository.GetAllWithStudentAndProgramAsync();

            if (onlyActive)
                dropouts = dropouts.Where(d => d.Status == Status.Active);

            var responses = dropouts.Select(MapToResponse);
            return new SuccessResponse<IEnumerable<DropoutResponse>> { Data = responses };
        }

        public async Task<Response> GetByProgramIdAsync(int programId, bool onlyActive = false)
        {
            var dropouts = await _dropoutRepository.GetByProgramWithStudentAndProgramAsync(programId);

            if (onlyActive)
                dropouts = dropouts.Where(d => d.Status == Status.Active);

            var responses = dropouts.Select(MapToResponse);
            return new SuccessResponse<IEnumerable<DropoutResponse>> { Data = responses };
        }

        public override async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var dropout = await _dropoutRepository.GetByIdWithStudentAndProgramAsync(Id);

            if (dropout == null)
                return (false, new ErrorResponse { Errors = ["Dropout request not found."] });

            var response = MapToResponse(dropout);
            return (true, new SuccessResponse<DropoutResponse> { Data = response });
        }

        public async Task<(bool Success, Response Response)> GetStudentDropoutByProgramIdAsync(int programId)
        {
            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
                return (false, new ErrorResponse { Errors = ["User not authenticated."] });

            var dropout = await _dropoutRepository.GetByProgramAndStudentAsync(programId, studentId);

            if (dropout == null)
                return (false, new ErrorResponse { Errors = ["Dropout request not found."] });

            var response = MapToResponse(dropout);
            return (true, new SuccessResponse<DropoutResponse> { Data = response });
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(DropoutRequest request)
        {
            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            // Validate program exists
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null)
                return (0, new ErrorResponse { Errors = ["Program not found."] });

            var existingDropout = await _dropoutRepository.GetByProgramAndStudentAsync(request.ProgramId, studentId);
            if (existingDropout != null && existingDropout.ApplicationStatus == ApplicationStatus.Pending)
                return (0, new ErrorResponse { Errors = ["You already have a pending dropout request for this program."] });

            var dropout = request.Adapt<Dropout>();
            dropout.StudentId = studentId;

            var rows = await _dropoutRepository.AddAsync(dropout);

            if (rows > 0)
            {
                var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAndProgramAsync(dropout.Id);

                if (dropoutWithStudent != null)
                {
                    await _notificationService.SendDropoutRequestedAsync(dropoutWithStudent);
                    var response = MapToResponse(dropoutWithStudent);
                    return (rows, new SuccessResponse<DropoutResponse> { Data = response });
                }
            }

            return (rows, new ErrorResponse { Errors = ["Failed to create dropout request."] });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, DropoutRequest request)
        {
            var dropout = await _dropoutRepository.GetByIdAsync(ID);

            if (dropout == null)
                return (0, new ErrorResponse { Errors = ["Dropout request not found."] });

            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId) || dropout.StudentId != studentId)
                return (0, new ErrorResponse { Errors = ["Not authorized to update this dropout request."] });

            if (dropout.ApplicationStatus != ApplicationStatus.Pending)
                return (0, new ErrorResponse { Errors = ["Only pending dropout requests can be updated."] });

            // Validate program exists
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null)
                return (0, new ErrorResponse { Errors = ["Program not found."] });

            request.Adapt(dropout);
            dropout.UpdatedAt = DateTime.UtcNow;

            var rows = await _dropoutRepository.UpdateAsync(dropout);

            if (rows > 0)
            {
                var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAndProgramAsync(ID);

                if (dropoutWithStudent != null)
                {
                    await _notificationService.SendDropoutUpdatedAsync(dropoutWithStudent);
                    var response = MapToResponse(dropoutWithStudent);
                    return (rows, new SuccessResponse<DropoutResponse> { Data = response });
                }
            }

            return (rows, new ErrorResponse { Errors = ["Failed to update dropout request."] });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
        {
            var dropout = await _dropoutRepository.GetByIdWithStudentAndProgramAsync(Id);

            if (dropout == null)
                return (0, new ErrorResponse { Errors = ["Dropout request not found."] });

            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId) || dropout.StudentId != studentId)
                return (0, new ErrorResponse { Errors = ["Not authorized to cancel this dropout request."] });

            if (dropout.ApplicationStatus != ApplicationStatus.Pending)
                return (0, new ErrorResponse { Errors = ["Only pending dropout requests can be cancelled."] });

            var rows = await _dropoutRepository.DeleteAsync(dropout);

            if (rows > 0)
                await _notificationService.SendDropoutCancelledAsync(dropout);

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> ReviewDropoutAsync(int requestId, ApplicationStatus newStatus, string feedback)
        {
            var request = await _dropoutRepository.GetByIdAsync(requestId);

            if (request == null)
                return (false, new ErrorResponse { Errors = ["Dropout request not found."] });

            if (request.ApplicationStatus != ApplicationStatus.Pending)
                return (false, new ErrorResponse { Errors = ["Only pending dropout requests can be reviewed."] });

            request.ApplicationStatus = newStatus;
            request.UpdatedAt = DateTime.UtcNow;

            await _dropoutRepository.UpdateAsync(request);

            var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAndProgramAsync(requestId);

            if (dropoutWithStudent != null)
            {
                if (newStatus == ApplicationStatus.Approved)
                {
                    // Remove student from all sections in the program and delete program enrollment
                    await RemoveStudentFromProgramSectionsAsync(dropoutWithStudent.StudentId, dropoutWithStudent.ProgramId);
                    await _notificationService.SendDropoutApprovedAsync(dropoutWithStudent);
                }
                else if (newStatus == ApplicationStatus.Rejected)
                {
                    await _notificationService.SendDropoutRejectedAsync(dropoutWithStudent, feedback);
                }

                var response = MapToResponse(dropoutWithStudent);
                return (true, new SuccessResponse<DropoutResponse> { Data = response });
            }

            return (false, new ErrorResponse { Errors = ["Failed to retrieve dropout request details."] });
        }

        private async Task RemoveStudentFromProgramSectionsAsync(string studentId, int programId)
        {
            // Get all student sections for this student in sections belonging to the program
            var studentSections = await _studentSectionRepository.GetByStudentAndProgramAsync(studentId, programId);

            foreach (var studentSection in studentSections)
            {
                // Get the section and decrease its current capacity
                var section = await _sectionRepository.GetByIdAsync(studentSection.SectionId);
                if (section != null && section.CurrentCapacity > 0)
                {
                    section.CurrentCapacity--;
                    await _sectionRepository.UpdateAsync(section);
                }

                await _studentSectionRepository.DeleteAsync(studentSection);
            }

            // Remove program enrollment
            var enrollment = await _programEnrollmentRepository.GetByProgramAndStudentAsync(programId, studentId);
            if (enrollment != null)
            {
                await _programEnrollmentRepository.DeleteAsync(enrollment);
            }
        }

        private DropoutResponse MapToResponse(Dropout dropout)
        {
            return new DropoutResponse
            {
                Id = dropout.Id,
                Reasons = dropout.Reasons,
                ApplicationStatus = dropout.ApplicationStatus,
                CreatedAt = dropout.CreatedAt,
                ProgramId = dropout.ProgramId,
                ProgramName = dropout.Program?.Name ?? string.Empty,
                StudentId = dropout.StudentId,
                StudentName = dropout.Student?.FullName ?? string.Empty
            };
        }
    }
}
