using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;

namespace Sconce.BLL.Services.Classes
{
    public class ProgramService : GenericService<ProgramRequest, ProgramResponse, Program>, IProgramService
    {
        private readonly IProgramRepository _programRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IExamRepository _examRepository;

        public ProgramService(IProgramRepository programRepository, UserManager<ApplicationUser> userManager, INotificationService notificationService, IHttpContextAccessor httpContextAccessor, IExamRepository examRepository) : base(programRepository)
        {
            _programRepository = programRepository;
            _userManager = userManager;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _examRepository = examRepository;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(ProgramRequest request)
        {
            // Validate ExamWriterInstructor if provided
            Instructor? examWriterInstructor = null;
            if (!string.IsNullOrEmpty(request.ExamWriterInstructorId))
            {
                var examWriter = await _userManager.FindByIdAsync(request.ExamWriterInstructorId);
                if (examWriter == null || !(examWriter is Instructor))
                    return (0, new ErrorResponse { Errors = ["Exam writer instructor not found."] });
                
                examWriterInstructor = (Instructor)examWriter;
            }

            // Validate EvaluatorInstructor if provided
            Instructor? evaluatorInstructor = null;
            if (!string.IsNullOrEmpty(request.EvaluatorInstructorId))
            {
                var evaluator = await _userManager.FindByIdAsync(request.EvaluatorInstructorId);
                if (evaluator == null || !(evaluator is Instructor))
                    return (0, new ErrorResponse { Errors = ["Evaluator instructor not found."] });
                
                evaluatorInstructor = (Instructor)evaluator;
            }

            // Create the program
            var program = request.Adapt<Program>();
            var rows = await _programRepository.AddAsync(program);

            // Send notifications to assigned instructors
            if (examWriterInstructor != null)
            {
                await _notificationService.SendExamWriterAssignedAsync(examWriterInstructor, program.Name);
            }

            if (evaluatorInstructor != null)
            {
                await _notificationService.SendEvaluatorAssignedAsync(evaluatorInstructor, program.Name);
            }

            return (rows, new SuccessResponse<ProgramResponse> { Data = program.Adapt<ProgramResponse>() });
        }

        public async Task<(int NumberOfEntries, Response Response)> IncreasePlannedLevelCountAsync(int programId, IncreasePlannedCountRequest request)
        {
            // Get the program to update
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (0, new ErrorResponse { Errors = ["Program not found."] });

            // Increase the planned level count
            program.PlannedLevelCount += request.Increment;
            program.UpdatedAt = DateTime.UtcNow;

            var rows = await _programRepository.UpdateAsync(program);

            return (rows, new SuccessResponse<string> { Data = $"Planned level count increased to {program.PlannedLevelCount}." });
        }

        public async Task<(bool Success, Response Response)> AssignExamWriterInstructorAsync(int programId, string instructorId)
        {
            // Load program by id
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Check if program uses proficiency exam
            if (!program.HasProficiencyExam)
                return (false, new ErrorResponse { Errors = ["This program does not use a proficiency exam."] });

            // Validate instructor exists and has Instructor role
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null || !(instructor is Instructor))
                return (false, new ErrorResponse { Errors = ["Instructor not found."] });

            // Set the exam writer instructor
            program.ExamWriterInstructorId = instructorId;
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);

            // Notify instructor of assignment
            var instructorEntity = (Instructor)instructor;
            await _notificationService.SendExamWriterAssignedAsync(instructorEntity, program.Name);

            var response = new SuccessResponse<ProgramResponse>
            {
                Data = program.Adapt<ProgramResponse>()
            };

            return (true, response);
        }

        public async Task<(bool Success, Response Response)> AssignEvaluatorInstructorAsync(int programId, string instructorId)
        {
            // Load program by id
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Check if program uses proficiency exam
            if (!program.HasProficiencyExam)
                return (false, new ErrorResponse { Errors = ["This program does not use a proficiency exam."] });

            // Validate instructor exists and has Instructor role
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null || !(instructor is Instructor))
                return (false, new ErrorResponse { Errors = ["Instructor not found."] });

            // Set the evaluator instructor
            program.EvaluatorInstructorId = instructorId;
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);

            // Notify instructor of assignment
            var instructorEntity = (Instructor)instructor;
            await _notificationService.SendEvaluatorAssignedAsync(instructorEntity, program.Name);

            var response = new SuccessResponse<ProgramResponse>
            {
                Data = program.Adapt<ProgramResponse>()
            };

            return (true, response);
        }

        public async Task<Response> GetProgramsForExamWriterAsync()
        {
            // Extract instructor ID from claims
            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return new ErrorResponse { Errors = ["User not authenticated."] };

            // Get programs assigned to this instructor as exam writer
            var programs = await _programRepository.GetProgramsByExamWriterAsync(instructorId);

            // Map to response DTOs
            var programResponses = programs.Adapt<IEnumerable<ProgramResponse>>();

            return new SuccessResponse<IEnumerable<ProgramResponse>> { Data = programResponses };
        }

        public async Task<Response> GetProgramsForEvaluatorAsync()
        {
            // Extract instructor ID from claims
            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return new ErrorResponse { Errors = ["User not authenticated."] };

            // Get programs assigned to this instructor as evaluator
            var programs = await _programRepository.GetProgramsByEvaluatorAsync(instructorId);

            // Map to response DTOs
            var programResponses = programs.Adapt<IEnumerable<ProgramResponse>>();

            return new SuccessResponse<IEnumerable<ProgramResponse>> { Data = programResponses };
        }

        public async Task<(bool Success, Response Response)> AssignProficiencyExamAsync(int programId, int examId)
        {
            // Extract instructor ID
            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (false, new ErrorResponse { Errors = ["User not authenticated."] });

            // Load program
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Program not found."] });

            // Validate program settings
            if (!program.HasProficiencyExam)
                return (false, new ErrorResponse { Errors = ["This program does not have a proficiency exam enabled."] });
            
            if (program.ExamWriterInstructorId != instructorId)
                return (false, new ErrorResponse { Errors = ["Forbidden. You are not assigned as the exam writer for this program."] });

            if (program.ProficiencyExamId.HasValue)
                return (false, new ErrorResponse { Errors = ["Proficiency exam already assigned for this program."] });
            
            // Load exam
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            if (exam.ProgramId != programId)
                return (false, new ErrorResponse { Errors = ["Exam does not belong to this program."] });

            // Assign exam
            program.ProficiencyExamId = examId;
            program.UpdatedAt = DateTime.UtcNow;

            await _programRepository.UpdateAsync(program);

            var response = new SuccessResponse<ProgramResponse>
            {
                Data = program.Adapt<ProgramResponse>()
            };

            return (true, response);
        }
    }
}
