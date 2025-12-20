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
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DropoutService(
            IDropoutRepository dropoutRepository,
            INotificationService notificationService,
            IHttpContextAccessor httpContextAccessor) : base(dropoutRepository)
        {
            _dropoutRepository = dropoutRepository;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(DropoutRequest request)
        {
            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            var dropout = request.Adapt<Dropout>();
            dropout.StudentId = studentId;

            var rows = await _dropoutRepository.AddAsync(dropout);

            if (rows > 0)
            {
                var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAsync(dropout.Id);

                if (dropoutWithStudent != null)
                    await _notificationService.SendDropoutRequestedAsync(dropoutWithStudent);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
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

            request.Adapt(dropout);
            dropout.UpdatedAt = DateTime.UtcNow;

            var rows = await _dropoutRepository.UpdateAsync(dropout);

            if (rows > 0)
            {
                var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAsync(ID);

                if (dropoutWithStudent != null)
                    await _notificationService.SendDropoutUpdatedAsync(dropoutWithStudent);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
        {
            var dropout = await _dropoutRepository.GetByIdWithStudentAsync(Id);

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

            var dropoutWithStudent = await _dropoutRepository.GetByIdWithStudentAsync(requestId);

            if (dropoutWithStudent != null)
            {
                if (newStatus == ApplicationStatus.Approved)
                    await _notificationService.SendDropoutApprovedAsync(dropoutWithStudent);
                else if (newStatus == ApplicationStatus.Rejected)
                    await _notificationService.SendDropoutRejectedAsync(dropoutWithStudent, feedback);
            }

            // remove student from the program if approved

            var message = newStatus == ApplicationStatus.Approved
                ? "Dropout request approved."
                : "Dropout request rejected.";

            return (true, new SuccessResponse<string> { Data = message });
        }
    }
}
