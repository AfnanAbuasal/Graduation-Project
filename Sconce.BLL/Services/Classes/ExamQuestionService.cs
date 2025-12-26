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
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ExamQuestionService : GenericService<ExamQuestionRequest, ExamQuestionResponse, ExamQuestion>, IExamQuestionService
    {
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IExamRepository _examRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamQuestionService(
            IExamQuestionRepository examQuestionRepository,
            IExamRepository examRepository,
            IQuestionRepository questionRepository,
            ISectionRepository sectionRepository,
            IChoiceRepository choiceRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(examQuestionRepository)
        {
            _examQuestionRepository = examQuestionRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _sectionRepository = sectionRepository;
            _choiceRepository = choiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(ExamQuestionRequest request)
        {
            // Exam must exist
            var exam = await _examRepository.GetByIdAsync(request.ExamId);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            // Exam must be editable (Draft only)
            if (exam.ExamStatus != ExamStatus.Draft)
                return (0, new ErrorResponse { Errors = ["Cannot modify questions after publishing the exam."] });

            // Question must exist
            var question = await _questionRepository.GetByIdAsync(request.QuestionId);
            if (question == null)
                return (0, new ErrorResponse { Errors = ["Question not found."] });

            // Question must belong to the exam's course
            var section = await _sectionRepository.GetByIdAsync(exam.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found for this exam."] });

            if (question.CourseId != section.CourseId)
                return (0, new ErrorResponse { Errors = ["Question does not belong to this course."] });

            // Prevent duplicates (ExamId, QuestionId)
            var duplicateQuestion = await _examQuestionRepository.ExistsQuestionInExamAsync(request.ExamId, request.QuestionId);
            if (duplicateQuestion)
                return (0, new ErrorResponse { Errors = ["Question already added to this exam."] });

            // SortOrder uniqueness within exam
            var duplicateSortOrder = await _examQuestionRepository.ExistsSortOrderInExamAsync(request.ExamId, request.SortOrder);
            if (duplicateSortOrder)
                return (0, new ErrorResponse { Errors = [$"SortOrder {request.SortOrder} is already used in this exam."] });

            // Points validation (> 0)
            if (request.Points <= 0)
                return (0, new ErrorResponse { Errors = ["Points must be greater than 0."] });

            var entity = request.Adapt<ExamQuestion>();

            var rows = await _examQuestionRepository.AddAsync(entity);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public async Task<Response> GetAllByExamIdAsync(int examId)
        {
            // Validate exam exists
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return new ErrorResponse { Errors = ["Exam not found."] };

            var items = await _examQuestionRepository.GetAllByExamIdAsync(examId);
            return new SuccessResponse<IEnumerable<ExamQuestionResponse>> { Data = items.Adapt<IEnumerable<ExamQuestionResponse>>() };
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ExamQuestionRequest request)
        {
            // Fetch existing ExamQuestion
            var existing = await _examQuestionRepository.GetByIdAsync(id);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["ExamQuestion not found."] });

            // Fetch exam and validate Draft status
            var exam = await _examRepository.GetByIdAsync(existing.ExamId);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            if (exam.ExamStatus != ExamStatus.Draft)
                return (0, new ErrorResponse { Errors = ["Cannot modify questions after publishing the exam."] });

            // Validate question exists
            var question = await _questionRepository.GetByIdAsync(request.QuestionId);
            if (question == null)
                return (0, new ErrorResponse { Errors = ["Question not found."] });

            // Validate course match
            var section = await _sectionRepository.GetByIdAsync(exam.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found for this exam."] });

            if (question.CourseId != section.CourseId)
                return (0, new ErrorResponse { Errors = ["Question does not belong to this course."] });

            // Prevent duplicates when changing QuestionId
            var duplicateQuestion = await _examQuestionRepository.ExistsQuestionInExamAsync(existing.ExamId, request.QuestionId, excludeId: existing.Id);
            if (duplicateQuestion)
                return (0, new ErrorResponse { Errors = ["Question already added to this exam."] });

            // SortOrder uniqueness within exam (exclude current)
            var duplicateSortOrder = await _examQuestionRepository.ExistsSortOrderInExamAsync(existing.ExamId, request.SortOrder, excludeId: existing.Id);
            if (duplicateSortOrder)
                return (0, new ErrorResponse { Errors = [$"SortOrder {request.SortOrder} is already used in this exam."] });

            // Points validation (> 0)
            if (request.Points <= 0)
                return (0, new ErrorResponse { Errors = ["Points must be greater than 0."] });

            // Apply request fields; protect Id and ExamId (do not change exam association here)
            existing.QuestionId = request.QuestionId;
            existing.SortOrder = request.SortOrder;
            existing.Points = request.Points;
            existing.PromptOverride = request.PromptOverride;

            existing.UpdatedAt = DateTime.UtcNow;

            var rows = await _examQuestionRepository.UpdateAsync(existing);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
        {
            var existing = await _examQuestionRepository.GetByIdAsync(id);
            if (existing == null)
                return (0, new ErrorResponse { Errors = ["ExamQuestion not found."] });

            var exam = await _examRepository.GetByIdAsync(existing.ExamId);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            if (exam.ExamStatus != ExamStatus.Draft)
                return (0, new ErrorResponse { Errors = ["Cannot modify questions after publishing the exam."] });

            var rows = await _examQuestionRepository.DeleteAsync(existing);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> ReorderAsync(int examId, List<(int ExamQuestionId, int SortOrder)> newOrder)
        {
            // Validate exam exists and status
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            if (exam.ExamStatus != ExamStatus.Draft)
                return (false, new ErrorResponse { Errors = ["Cannot modify questions after publishing the exam."] });

            if (newOrder == null || newOrder.Count == 0)
                return (false, new ErrorResponse { Errors = ["No reorder payload provided."] });

            // Ensure payload SortOrders are unique
            var sortOrders = newOrder.Select(x => x.SortOrder).ToList();
            if (sortOrders.Count != sortOrders.Distinct().Count())
                return (false, new ErrorResponse { Errors = ["SortOrder values in payload must be unique."] });

            // Fetch existing EQs for this exam
            var eqs = (await _examQuestionRepository.GetAllByExamIdAsync(examId)).ToList();
            var eqIds = eqs.Select(e => e.Id).ToHashSet();

            // Ensure all provided IDs belong to this exam
            var providedIds = newOrder.Select(x => x.ExamQuestionId).ToList();
            if (!providedIds.All(id => eqIds.Contains(id)))
                return (false, new ErrorResponse { Errors = ["One or more provided ExamQuestionId do not belong to this exam."] });

            // Prevent collisions with questions not included in the payload
            var untouchedSortOrders = eqs
                .Where(eq => !providedIds.Contains(eq.Id))
                .Select(eq => eq.SortOrder)
                .ToHashSet();

            if (newOrder.Any(o => untouchedSortOrders.Contains(o.SortOrder)))
                return (false, new ErrorResponse { Errors = ["SortOrder values conflict with existing questions not in the payload."] });

            // Apply new sort orders
            // Step 1: move affected rows to temporary high numbers to satisfy unique index during swap
            var rowsAffected = 0;
            var tempBase = 1_000_000;
            var tempIncrement = 0;

            foreach (var (ExamQuestionId, _) in newOrder)
            {
                var entity = eqs.First(e => e.Id == ExamQuestionId);
                entity.SortOrder = tempBase + tempIncrement++;
                entity.UpdatedAt = DateTime.UtcNow;
                rowsAffected += await _examQuestionRepository.UpdateAsync(entity);
            }

            // Step 2: apply the desired ordering
            foreach (var (ExamQuestionId, SortOrder) in newOrder)
            {
                var entity = eqs.First(e => e.Id == ExamQuestionId);
                entity.SortOrder = SortOrder;
                entity.UpdatedAt = DateTime.UtcNow;
                rowsAffected += await _examQuestionRepository.UpdateAsync(entity);
            }

            return (true, new SuccessResponse<string> { Data = "Exam questions reordered successfully." });
        }

        public async Task<Response> GetAllExamQuestionDetailsForInstructorAsync(int examId)
        {
            return await GetAllExamQuestionDetailsAsync(examId, includeCorrectAnswers: true, forStudent: false);
        }

        public async Task<Response> GetAllExamQuestionDetailsForStudentAsync(int examId)
        {
            return await GetAllExamQuestionDetailsAsync(examId, includeCorrectAnswers: false, forStudent: true);
        }

        private async Task<Response> GetAllExamQuestionDetailsAsync(int examId, bool includeCorrectAnswers, bool forStudent)
        {
            // Load exam by id
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                return new ErrorResponse { Errors = ["Exam not found."] };

            // If forStudent: validate ExamStatus and availability window
            if (forStudent)
            {
                if (exam.ExamStatus != ExamStatus.Published)
                    return new ErrorResponse { Errors = ["Exam is not published yet."] };

                var now = DateTime.UtcNow;

                if (exam.AvailableFrom.HasValue && now < exam.AvailableFrom.Value)
                    return new ErrorResponse { Errors = ["Exam not available yet."] };

                if (exam.AvailableTo.HasValue && now > exam.AvailableTo.Value)
                    return new ErrorResponse { Errors = ["Exam has ended."] };
            }

            // Load examQuestions with Question included (ordered by SortOrder)
            var examQuestions = await _examQuestionRepository.GetAllDetailsByExamIdAsync(examId);

            if (!examQuestions.Any())
                return new SuccessResponse<List<ExamQuestionDetailsResponse>> { Data = new List<ExamQuestionDetailsResponse>() };

            // Identify MCQ question IDs
            var mcqQuestionIds = examQuestions
                .Where(eq => eq.Question.Type == "MultipleChoiceQuestion")
                .Select(eq => eq.QuestionId)
                .Distinct()
                .ToList();

            // Load choices in one DB query for all MCQ questions
            var allChoices = new List<Choice>();
            if (mcqQuestionIds.Any())
            {
                allChoices = (List<Choice>)await _choiceRepository.GetByQuestionIdsAsync(mcqQuestionIds);
            }

            // Group choices by QuestionId for fast lookup
            var choicesByQuestionId = allChoices.GroupBy(c => c.QuestionId)
                .ToDictionary(g => g.Key, g => g.ToList());

            // Map each ExamQuestion into ExamQuestionDetailsResponse
            var result = examQuestions.Select(eq =>
            {
                var question = eq.Question;
                var promptToUse = !string.IsNullOrWhiteSpace(eq.PromptOverride) ? eq.PromptOverride : question.Prompt;

                var questionResponse = new QuestionResponse
                {
                    Id = question.Id,
                    Prompt = promptToUse,
                    Difficulty = question.Difficulty,
                    CreatedByInstructorId = question.CreatedByInstructorId,
                    CourseId = question.CourseId,
                    Type = question.Type,
                    CreatedAt = question.CreatedAt,
                    Status = question.Status
                };

                var examQuestionResponse = new ExamQuestionResponse
                {
                    Id = eq.Id,
                    ExamId = eq.ExamId,
                    QuestionId = eq.QuestionId,
                    SortOrder = eq.SortOrder,
                    Points = eq.Points,
                    PromptOverride = eq.PromptOverride,
                    CreatedAt = eq.CreatedAt,
                    UpdatedAt = eq.UpdatedAt
                };

                var detailsResponse = new ExamQuestionDetailsResponse
                {
                    ExamQuestion = examQuestionResponse,
                    Question = questionResponse
                };

                // Add choices for MCQ questions
                if (question.Type == "MultipleChoiceQuestion" && choicesByQuestionId.ContainsKey(question.Id))
                {
                    var choices = choicesByQuestionId[question.Id];
                    detailsResponse.Choices = choices.Select(c => new ChoiceResponse
                    {
                        QuestionId = c.QuestionId,
                        Text = c.Text,
                        IsCorrect = includeCorrectAnswers ? c.IsCorrect : false
                    }).ToList();
                }

                return detailsResponse;
            }).ToList();

            // Return SuccessResponse
            return new SuccessResponse<List<ExamQuestionDetailsResponse>> { Data = result };
        }
    }
}