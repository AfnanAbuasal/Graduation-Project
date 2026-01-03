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
using System.Text.Json;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ExamAttemptService : IExamAttemptService
    {
        private readonly IExamAttemptRepository _examAttemptRepository;
        private readonly IExamRepository _examRepository;
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IExamQuestionService _examQuestionService;
        private readonly IAnswerRepository _answerRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUrlHelper _urlHelper;

        public ExamAttemptService(
            IExamAttemptRepository examAttemptRepository,
            IExamRepository examRepository,
            IExamQuestionRepository examQuestionRepository,
            IExamQuestionService examQuestionService,
            IAnswerRepository answerRepository,
            IHttpContextAccessor httpContextAccessor,
            IUrlHelper urlHelper)
        {
            _examAttemptRepository = examAttemptRepository;
            _examRepository = examRepository;
            _examQuestionRepository = examQuestionRepository;
            _examQuestionService = examQuestionService;
            _answerRepository = answerRepository;
            _httpContextAccessor = httpContextAccessor;
            _urlHelper = urlHelper;
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

            var existingAttempt = await _examAttemptRepository.GetInProgressAttemptAsync(exam.Id, studentId);
            if (existingAttempt != null)
                return (true, new SuccessResponse<ExamAttemptResponse> { Data = MapToResponse(existingAttempt) });
            
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

            // Reload to ensure Student navigation is available for mapping FullName
            var persistedAttempt = await _examAttemptRepository.GetByIdWithExamAsync(attempt.Id) ?? attempt;

            return (true, new SuccessResponse<ExamAttemptResponse> { Data = MapToResponse(persistedAttempt) });
        }

        public async Task<Response> GetMyAttemptsAsync(int examId)
        {
            var studentId = GetCurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return new ErrorResponse { Errors = ["Not authenticated."] };

            var attempts = await _examAttemptRepository.GetAttemptsByExamForStudentAsync(examId, studentId);
            var response = attempts.Select(MapToResponseWithAnswers).ToList();

            return new SuccessResponse<IEnumerable<ExamAttemptResponse>> { Data = response };
        }

        public async Task<Response> GetAttemptsByExamIdAsync(int examId)
        {
            var instructorId = GetCurrentInstructorId();
            if (string.IsNullOrEmpty(instructorId))
                return new ErrorResponse { Errors = ["Not authenticated."] };

            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return new ErrorResponse { Errors = ["Exam not found."] };

            var attempts = await _examAttemptRepository.GetAllByExamIdAsync(examId);
            var response = attempts.Select(MapToResponseWithAnswers).ToList();

            return new SuccessResponse<IEnumerable<ExamAttemptResponse>> { Data = response };
        }

        public async Task<(bool Success, Response Response)> GetAttemptDetailsAsync(int attemptId)
        {
            var instructorId = GetCurrentInstructorId();
            if (string.IsNullOrEmpty(instructorId))
                return (false, new ErrorResponse { Errors = ["Not authenticated."] });

            // Load attempt with basic data (exam, student, answers)
            var attempt = await _examAttemptRepository.GetByIdWithExamAsync(attemptId);
            if (attempt == null)
                return (false, new ErrorResponse { Errors = ["Attempt not found."] });

            if (attempt.Exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found for this attempt."] });

            // Map basic attempt info with answers
            var attemptResponse = MapToResponseWithAnswers(attempt);

            // Use ExamQuestionService to load full question details separately
            var questionsResult = await _examQuestionService.GetAllExamQuestionDetailsAsync(attempt.ExamId, forStudents: false);
            var questions = new List<ExamQuestionDetailsResponse>();

            if (questionsResult is SuccessResponse<List<ExamQuestionDetailsResponse>> successResponse)
            {
                questions = successResponse.Data ?? new List<ExamQuestionDetailsResponse>();
            }

            var detailsResponse = new ExamAttemptDetailsResponse
            {
                Attempt = attemptResponse,
                Questions = questions
            };

            return (true, new SuccessResponse<ExamAttemptDetailsResponse> { Data = detailsResponse });
        }

        public async Task<(bool Success, Response Response)> SubmitAttemptAsync(int attemptId)
        {
            var studentId = GetCurrentStudentId();
            if (string.IsNullOrEmpty(studentId))
                return (false, new ErrorResponse { Errors = ["Not authenticated."] });

            var attempt = await _examAttemptRepository.GetByIdWithExamAsync(attemptId);
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

            // Reload to include latest answers with question data
            var refreshedAttempt = await _examAttemptRepository.GetByIdWithExamAsync(attempt.Id) ?? attempt;

            return (true, new SuccessResponse<ExamAttemptResponse> { Data = MapToResponseWithAnswers(refreshedAttempt) });
        }

        private string? GetCurrentStudentId()
            => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private string? GetCurrentInstructorId()
            => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        private ExamAttemptResponse MapToResponse(ExamAttempt attempt)
        {
            var dto = attempt.Adapt<ExamAttemptResponse>();
            dto.StudentFullName = attempt.Student?.FullName ?? string.Empty;
            return dto;
        }

        private ExamAttemptResponse MapToResponseWithAnswers(ExamAttempt attempt)
        {
            var dto = MapToResponse(attempt);

            var answers = attempt.Answers;

            // If answers not loaded, fetch explicitly
            if (answers == null || !answers.Any())
            {
                var loaded = _answerRepository.GetAllByAttemptIdAsync(attempt.Id).GetAwaiter().GetResult();
                answers = loaded?.ToList() ?? new List<Answer>();
            }

            dto.Answers = answers
                .Select(MapAnswerToResponse)
                .ToList();

            return dto;
        }

        private AnswerResponse MapAnswerToResponse(Answer answer)
        {
            var dto = answer.Adapt<AnswerResponse>();

            // Parse SelectedChoiceIdsJson => List<int>
            if (!string.IsNullOrWhiteSpace(answer.SelectedChoiceIdsJson))
            {
                try
                {
                    dto.SelectedChoiceIds = JsonSerializer.Deserialize<List<int>>(answer.SelectedChoiceIdsJson);
                }
                catch
                {
                    dto.SelectedChoiceIds = null;
                }
            }

            // Build FileUrl only for essay question answers
            if (answer.ExamQuestion?.Question is EssayQuestion && !string.IsNullOrWhiteSpace(answer.FilePath))
            {
                dto.FileUrl = _urlHelper.BuildUrl(answer.FilePath);
            }

            return dto;
        }
    }
}
