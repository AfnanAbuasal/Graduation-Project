using Mapster;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Sconce.DAL.Models.Enums;

namespace Sconce.BLL.Services.Classes
{
    public class AnswerService : FileGenericService<AnswerRequest, AnswerResponse, Answer>, IAnswerService
    {
        private readonly IAnswerRepository _answerRepository;
        private readonly IExamAttemptRepository _examAttemptRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IFileService _fileService;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnswerService(
            IAnswerRepository answerRepository,
            IExamAttemptRepository examAttemptRepository,
            IChoiceRepository choiceRepository,
            IFileService fileService,
            IUrlHelper urlHelper,
            IHttpContextAccessor httpContextAccessor)
            : base(answerRepository, fileService, urlHelper, "Uploads/EssayAnswers")
        {
            _answerRepository = answerRepository;
            _examAttemptRepository = examAttemptRepository;
            _choiceRepository = choiceRepository;
            _fileService = fileService;
            _urlHelper = urlHelper;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(AnswerRequest request)
        {
            // Get studentId from JWT claims
            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
                return (0, new ErrorResponse { Errors = ["Not authenticated."] });

            // Load attempt with exam info
            var attempt = await _examAttemptRepository.GetByIdWithExamAsync(request.ExamAttemptId);

            if (attempt == null)
                return (0, new ErrorResponse { Errors = ["Attempt not found."] });

            if (attempt.StudentId != studentId)
                return (0, new ErrorResponse { Errors = ["Unauthorized Access."] });

            if (attempt.AttemptStatus != AttemptStatus.InProgress)
                return (0, new ErrorResponse { Errors = ["Attempt is not in progress."] });

            // Check if exam duration expired
            if (attempt.ExpiresAt != null && DateTime.UtcNow >= attempt.ExpiresAt)
            {
                attempt.AttemptStatus = AttemptStatus.Expired;
                await _examAttemptRepository.UpdateAsync(attempt);
                return (0, new ErrorResponse { Errors = ["Exam duration expired."] });
            }

            // Validate examQuestion belongs to attempt.ExamId
            var examQuestion = await _answerRepository.GetExamQuestionWithMcqChoicesAsync(request.ExamQuestionId);

            if (examQuestion == null)
                return (0, new ErrorResponse { Errors = ["Exam question not found."] });

            if (examQuestion.ExamId != attempt.ExamId)
                return (0, new ErrorResponse { Errors = ["Exam question does not belong to this attempt's exam."] });

            // Check if answer already exists
            var existingAnswer = await _answerRepository.GetByAttemptAndExamQuestionAsync(attempt.Id, examQuestion.Id);

            Answer answer;
            bool isUpdate = false;

            if (existingAnswer != null)
            {
                answer = existingAnswer;
                isUpdate = true;
            }
            else
            {
                answer = new Answer
                {
                    ExamAttemptId = attempt.Id,
                    ExamQuestionId = examQuestion.Id
                };
            }

            // Set MaxScore from ExamQuestion.Points (snapshot at time of answering)
            answer.MaxScore = examQuestion.Points;

            // Handle based on question type
            if (examQuestion.Question is MultipleChoiceQuestion mcQuestion)
            {
                var selectedIds = request.SelectedChoiceIds?
                    .Distinct()
                    .ToList();

                if (selectedIds == null || selectedIds.Count == 0)
                    return (0, new ErrorResponse { Errors = ["Please select at least one choice for multiple choice question."] });

                var mcChoices = mcQuestion.Choices?.ToList() ?? new List<Choice>();

                // Fallback to repository if choices were not preloaded
                if (!mcChoices.Any())
                    mcChoices = (await _choiceRepository.GetByQuestionIdAsync(examQuestion.QuestionId)).ToList();

                var validChoiceIds = mcChoices.Select(c => c.Id).ToHashSet();

                foreach (var choiceId in selectedIds)
                {
                    if (!validChoiceIds.Contains(choiceId))
                        return (0, new ErrorResponse { Errors = [$"Invalid choice ID: {choiceId}"] });
                }

                var correctChoiceIds = mcChoices.Where(c => c.IsCorrect).Select(c => c.Id).ToHashSet();

                decimal score = 0m;

                if (!mcQuestion.AllowMultipleSelections)
                {
                    if (selectedIds.Count == 1 && correctChoiceIds.Contains(selectedIds[0]))
                        score = examQuestion.Points;
                }
                else
                {
                    var hasWrongSelection = selectedIds.Any(id => !correctChoiceIds.Contains(id));

                    if (!hasWrongSelection)
                    {
                        var correctCount = correctChoiceIds.Count;

                        if (correctCount > 0)
                        {
                            score = (examQuestion.Points / correctCount) * selectedIds.Count;

                            if (score > examQuestion.Points)
                                score = examQuestion.Points;
                        }
                    }
                }

                answer.Score = score;
                answer.GradedAt = DateTime.UtcNow;
                answer.GradedByInstructorId = null;

                // Store as JSON with de-duplicated selected IDs
                answer.SelectedChoiceIdsJson = JsonSerializer.Serialize(selectedIds);
                answer.Text = null;

                // Delete old file if exists
                if (!string.IsNullOrEmpty(answer.FilePath))
                {
                    await _fileService.DeleteFileAsync(answer.FilePath);
                    answer.FilePath = null;
                }
            }
            else if (examQuestion.Question is EssayQuestion)
            {
                // Essay validation: require Text and/or File
                if (string.IsNullOrWhiteSpace(request.Text) && request.File == null)
                    return (0, new ErrorResponse { Errors = ["Please provide a text answer or upload a file."] });

                answer.Text = request.Text;

                // Handle file upload
                if (request.File != null)
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(answer.FilePath))
                        await _fileService.DeleteFileAsync(answer.FilePath);

                    answer.FilePath = await _fileService.SaveFileAsync(request.File, "Uploads/EssayAnswers");
                }

                answer.SelectedChoiceIdsJson = null;
            }
            else
            {
                return (0, new ErrorResponse { Errors = ["Unknown question type."] });
            }

            int rows;

            if (isUpdate)
            {
                answer.UpdatedAt = DateTime.UtcNow;
                rows = await _answerRepository.UpdateAsync(answer);
            }
            else
            {
                rows = await _answerRepository.AddAsync(answer);
            }

            if (rows > 0)
            {
                var response = MapToResponse(answer);
                return (rows, new SuccessResponse<AnswerResponse> { Data = response });
            }

            return (0, new ErrorResponse { Errors = ["Failed to save answer."] });
        }

        public async Task<Response> GetMyAnswersForAttemptAsync(int attemptId)
        {
            // Get studentId from claims
            var studentId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(studentId))
                return new ErrorResponse { Errors = ["Not authenticated."] };

            // Load attempt and verify owner
            var attempt = await _examAttemptRepository.GetByIdAsync(attemptId);

            if (attempt == null)
                return new ErrorResponse { Errors = ["Attempt not found."] };

            if (attempt.StudentId != studentId)
                return new ErrorResponse { Errors = ["Unauthorized."] };

            // Get all answers for this attempt
            var answers = await _answerRepository.GetAllByAttemptIdAsync(attemptId);

            var responseList = answers.Select(MapToResponse).ToList();

            return new SuccessResponse<IEnumerable<AnswerResponse>> { Data = responseList };
        }

        private AnswerResponse MapToResponse(Answer answer)
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

            // Build FileUrl
            dto.FileUrl = _urlHelper.BuildUrl(answer.FilePath);

            return dto;
        }

        public async Task<(bool Success, Response Response)> GradeEssayAnswerAsync(int answerId, decimal score)
        {
            // Get instructorId from JWT claims
            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(instructorId))
                return (false, new ErrorResponse { Errors = ["Not authenticated."] });

            // Load answer with relationships
            var answer = await _answerRepository.GetByIdWithAttemptAndQuestionAsync(answerId);

            if (answer == null)
                return (false, new ErrorResponse { Errors = ["Answer not found."] });

            // Verify attempt is in a state that can be graded
            if (answer.ExamAttempt.AttemptStatus != AttemptStatus.Submitted && 
                answer.ExamAttempt.AttemptStatus != AttemptStatus.Expired)
                return (false, new ErrorResponse { Errors = ["In-Progress attempts cannot be graded yet."] });

            // Ensure question is Essay
            if (!(answer.ExamQuestion.Question is EssayQuestion))
                return (false, new ErrorResponse { Errors = ["Only essay answers can be graded manually."] });
            // Validate score range
            if (score < 0)
                return (false, new ErrorResponse { Errors = ["Score cannot be negative."] });

            if (score > answer.MaxScore)
                return (false, new ErrorResponse { Errors = [$"Score cannot exceed maxScore ({answer.MaxScore})."] });

            // Apply grading
            answer.Score = score;
            answer.GradedAt = DateTime.UtcNow;
            answer.GradedByInstructorId = instructorId;
            answer.UpdatedAt = DateTime.UtcNow;

            var rows = await _answerRepository.UpdateAsync(answer);

            if (rows > 0)
            {
                var response = MapToResponse(answer);
                return (true, new SuccessResponse<AnswerResponse> { Data = response });
            }

            return (false, new ErrorResponse { Errors = ["Failed to grade answer."] });
        }
    }
}