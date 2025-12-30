using Mapster;
using Microsoft.AspNetCore.Http;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ExamAttemptService : IExamAttemptService
    {
        private readonly IExamAttemptRepository _examAttemptRepository;
        private readonly IExamRepository _examRepository;
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamAttemptService(
            IExamAttemptRepository examAttemptRepository,
            IExamRepository examRepository,
            IExamQuestionRepository examQuestionRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _examAttemptRepository = examAttemptRepository;
            _examRepository = examRepository;
            _examQuestionRepository = examQuestionRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<(bool Success, Response Response)> StartAttemptAsync(int examId)
        {
            var studentId = GetCurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return (false, new ErrorResponse { Errors = ["Not authenticated."] });

            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            if (exam.ExamStatus != ExamStatus.Published)
                return (false, new ErrorResponse { Errors = ["Exam is not published yet."] });

            var now = DateTime.UtcNow;

            if (exam.AvailableFrom.HasValue && now < exam.AvailableFrom.Value)
                return (false, new ErrorResponse { Errors = ["Exam not available yet."] });

            if (exam.AvailableTo.HasValue && now > exam.AvailableTo.Value)
                return (false, new ErrorResponse { Errors = ["Exam availability has ended."] });

            var existingAttempt = await _examAttemptRepository.GetInProgressAttemptAsync(exam.Id, studentId); //ToDo: include answers after they're implemented
            if (existingAttempt != null)
                return (true, new SuccessResponse<ExamAttemptResponse> { Data = existingAttempt.Adapt<ExamAttemptResponse>() });
            
            var attemptsCount = await _examAttemptRepository.GetAttemptsCountAsync(exam.Id, studentId);
            if (attemptsCount >= exam.AttemptsAllowed)
                return (false, new ErrorResponse { Errors = [$"You have reached the allowed {exam.AttemptsAllowed} attempts limit."] });

            var attemptNumber = attemptsCount + 1;
            var startedAt = now;
            var expiresAt = exam.DurationMinutes.HasValue
                ? startedAt.AddMinutes(exam.DurationMinutes.Value)
                : (DateTime?)null;

            var attempt = new ExamAttempt
            {
                ExamId = exam.Id,
                StudentId = studentId,
                AttemptNumber = attemptNumber,
                AttemptStatus = AttemptStatus.InProgress,
                StartedAt = startedAt,
                ExpiresAt = expiresAt
            };

            await _examAttemptRepository.AddAsync(attempt);

            return (true, new SuccessResponse<ExamAttemptResponse> { Data = attempt.Adapt<ExamAttemptResponse>() });
        }

        public async Task<Response> GetMyAttemptsAsync(int examId)
        {
            var studentId = GetCurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return new ErrorResponse { Errors = ["Not authenticated."] };

            var attempts = await _examAttemptRepository.GetAttemptsByExamForStudentAsync(examId, studentId);
            var response = attempts.Adapt<IEnumerable<ExamAttemptResponse>>();

            return new SuccessResponse<IEnumerable<ExamAttemptResponse>> { Data = response };
        }

        public async Task<(bool Success, Response Response)> SubmitAttemptAsync(int attemptId)
        {
            var studentId = GetCurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return (false, new ErrorResponse { Errors = ["Not authenticated."] });

            var attempt = await _examAttemptRepository.GetByIdWithExamAsync(attemptId); // ToDo: include answers after they're implemented
            if (attempt == null)
                return (false, new ErrorResponse { Errors = ["Attempt not found."] });
            
            if (!string.Equals(attempt.StudentId, studentId, StringComparison.Ordinal))
                return (false, new ErrorResponse { Errors = ["Unauthorized attempt access."] });

            if (attempt.AttemptStatus == AttemptStatus.Submitted)
                return (false, new ErrorResponse { Errors = ["Attempt has already been submitted."] });

            if (attempt.AttemptStatus != AttemptStatus.InProgress)
                return (false, new ErrorResponse { Errors = ["Attempt is not in progress."] });

            var now = DateTime.UtcNow;
            var timedOut = attempt.ExpiresAt.HasValue && now >= attempt.ExpiresAt.Value;

            var examQuestions = await _examQuestionRepository.GetAllByExamIdAsync(attempt.ExamId);
            attempt.MaxScore = examQuestions.Sum(eq => eq.Points);

            attempt.SubmittedAt = now;
            attempt.AttemptStatus = timedOut ? AttemptStatus.Expired : AttemptStatus.Submitted;
            attempt.UpdatedAt = now;

            await _examAttemptRepository.UpdateAsync(attempt);

            return (true, new SuccessResponse<ExamAttemptResponse> { Data = attempt.Adapt<ExamAttemptResponse>() });
        }

        private string? GetCurrentStudentId()
            => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}
