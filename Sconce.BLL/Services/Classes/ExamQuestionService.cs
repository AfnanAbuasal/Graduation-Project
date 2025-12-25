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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamQuestionService(
            IExamQuestionRepository examQuestionRepository,
            IExamRepository examRepository,
            IQuestionRepository questionRepository,
            ISectionRepository sectionRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(examQuestionRepository)
        {
            _examQuestionRepository = examQuestionRepository;
            _examRepository = examRepository;
            _questionRepository = questionRepository;
            _sectionRepository = sectionRepository;
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
            var eqs = await _examQuestionRepository.GetAllByExamIdAsync(examId);
            var eqIds = eqs.Select(e => e.Id).ToHashSet();

            // Ensure all provided IDs belong to this exam
            var providedIds = newOrder.Select(x => x.ExamQuestionId).ToList();
            if (!providedIds.All(id => eqIds.Contains(id)))
                return (false, new ErrorResponse { Errors = ["One or more provided ExamQuestionId do not belong to this exam."] });

            // Apply new sort orders
            var rowsAffected = 0;
            foreach (var (ExamQuestionId, SortOrder) in newOrder)
            {
                var entity = eqs.First(e => e.Id == ExamQuestionId);
                // No need to re-check uniqueness here since payload was validated; apply directly
                entity.SortOrder = SortOrder;
                entity.UpdatedAt = DateTime.UtcNow;
                rowsAffected += await _examQuestionRepository.UpdateAsync(entity);
            }

            return (true, new SuccessResponse<string> { Data = "Exam questions reordered successfully." });
        }
    }
}
