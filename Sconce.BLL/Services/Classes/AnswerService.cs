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
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IFileService _fileService;
        private readonly IUrlHelper _urlHelper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AnswerService(
            IAnswerRepository answerRepository,
            IExamAttemptRepository examAttemptRepository,
            IExamQuestionRepository examQuestionRepository,
            IChoiceRepository choiceRepository,
            IFileService fileService,
            IUrlHelper urlHelper,
            IHttpContextAccessor httpContextAccessor)
            : base(answerRepository, fileService, urlHelper, "Uploads/EssayAnswers")
        {
            _answerRepository = answerRepository;
            _examAttemptRepository = examAttemptRepository;
            _examQuestionRepository = examQuestionRepository;
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
            var examQuestion = await _examQuestionRepository.GetByIdWithQuestionAsync(request.ExamQuestionId);

            if (examQuestion == null)
                return (0, new ErrorResponse { Errors = ["Exam question not found."] });

            if (examQuestion.ExamId != attempt.ExamId)
                return (0, new ErrorResponse { Errors = ["Exam question does not belong to this attempt's exam."] });

            // Decide question type
            var questionType = examQuestion.Question.Type;

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
            if (questionType == "MultipleChoiceQuestion")
            {
                // MCQ validation
                if (request.SelectedChoiceIds == null || !request.SelectedChoiceIds.Any())
                    return (0, new ErrorResponse { Errors = ["Please select at least one choice for multiple choice question."] });

                // Validate all choiceIds belong to that questionId
                var allChoices = await _choiceRepository.GetByQuestionIdAsync(examQuestion.QuestionId);
                var validChoiceIds = allChoices.Select(c => c.Id).ToHashSet();

                foreach (var choiceId in request.SelectedChoiceIds)
                {
                    if (!validChoiceIds.Contains(choiceId))
                        return (0, new ErrorResponse { Errors = [$"Invalid choice ID: {choiceId}"] });
                }

                // Check if question allows multiple selections
                var mcQuestion = examQuestion.Question as MultipleChoiceQuestion;
                if (mcQuestion != null && !mcQuestion.AllowMultipleSelections && request.SelectedChoiceIds.Count > 1)
                    return (0, new ErrorResponse { Errors = ["This question does not allow multiple selections."] });

                // Store as JSON
                answer.SelectedChoiceIdsJson = JsonSerializer.Serialize(request.SelectedChoiceIds);
                answer.Text = null;

                // Delete old file if exists
                if (!string.IsNullOrEmpty(answer.FilePath))
                {
                    await _fileService.DeleteFileAsync(answer.FilePath);
                    answer.FilePath = null;
                }
            }
            else if (questionType == "EssayQuestion")
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
    }
}
