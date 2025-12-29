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
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ExamService : GenericService<ExamRequest, ExamResponse, Exam>, IExamService
    {
        private readonly IExamRepository _examRepository;
        private readonly ISectionRepository _sectionRepository;
        private readonly IExamQuestionRepository _examQuestionRepository;
        private readonly IChoiceRepository _choiceRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ExamService(
            IExamRepository examRepository,
            ISectionRepository sectionRepository,
            IExamQuestionRepository examQuestionRepository,
            IChoiceRepository choiceRepository,
            IHttpContextAccessor httpContextAccessor)
            : base(examRepository)
        {
            _examRepository = examRepository;
            _sectionRepository = sectionRepository;
            _examQuestionRepository = examQuestionRepository;
            _choiceRepository = choiceRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var exam = await _examRepository.GetByIdAsync(Id);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });
            
            var examResponse = exam.Adapt<ExamResponse>();

            return (true, new SuccessResponse<ExamResponse> { Data = examResponse });
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(ExamRequest request)
        {
            // Validate Section exists
            var section = await _sectionRepository.GetByIdAsync(request.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found."] });

            // Validate AttemptsAllowed >= 1
            if (request.AttemptsAllowed < 1)
                return (0, new ErrorResponse { Errors = ["AttemptsAllowed must be at least 1."] });

            // Validate DurationMinutes (null or > 0)
            if (request.DurationMinutes.HasValue && request.DurationMinutes.Value <= 0)
                return (0, new ErrorResponse { Errors = ["DurationMinutes must be greater than 0 when provided."] });

            // Validate availability window
            if (request.AvailableFrom.HasValue && request.AvailableTo.HasValue)
            {
                if (request.AvailableFrom >= request.AvailableTo)
                    return (0, new ErrorResponse { Errors = ["AvailableFrom must be before AvailableTo."] });
            }

            // Map request to entity
            var exam = request.Adapt<Exam>();

            // Force ExamStatus to Draft on creation
            exam.ExamStatus = ExamStatus.Draft;

            var rows = await _examRepository.AddAsync(exam);
            return (rows, new SuccessResponse<ExamResponse> { Data = exam.Adapt<ExamResponse>() });
        }

        public async Task<Response> GetAllBySectionAsync(int sectionId, string instructorId, bool onlyActive = false)
        {
            // Validate Section exists
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section == null)
                return new ErrorResponse { Errors = ["Section not found."] };

            // Verify section is assigned to the instructor
            if (section.InstructorId != instructorId)
                return new ErrorResponse { Errors = ["Unauthorized access to this section."] };

            // Get all exams for this section
            var exams = await _examRepository.GetAllBySectionIdAsync(sectionId, withTracking: false);

            if (onlyActive)
                exams = exams.Where(e => e.Status == Status.Active);

            // Map to ExamResponse and manually set CourseId
            var examResponses = exams.Adapt<IEnumerable<ExamResponse>>().ToList();

            return new SuccessResponse<IEnumerable<ExamResponse>> { Data = examResponses };
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ExamRequest request)
        {
            // Fetch existing exam
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            // Block updates if exam is closed
            if (exam.ExamStatus == ExamStatus.Closed)
                return (0, new ErrorResponse { Errors = ["Closed exams cannot be edited."] });

            // Validate Section exists
            var section = await _sectionRepository.GetByIdAsync(request.SectionId);
            if (section == null)
                return (0, new ErrorResponse { Errors = ["Section not found."] });

            // Validate AttemptsAllowed
            if (request.AttemptsAllowed < 1)
                return (0, new ErrorResponse { Errors = ["AttemptsAllowed must be at least 1."] });

            // Validate DurationMinutes
            if (request.DurationMinutes.HasValue && request.DurationMinutes.Value <= 0)
                return (0, new ErrorResponse { Errors = ["DurationMinutes must be greater than 0 when provided."] });

            // Validate availability window
            if (request.AvailableFrom.HasValue && request.AvailableTo.HasValue)
            {
                if (request.AvailableFrom >= request.AvailableTo)
                    return (0, new ErrorResponse { Errors = ["AvailableFrom must be before AvailableTo."] });
            }

            // If exam is published, only allow updating Title
            if (exam.ExamStatus == ExamStatus.Published)
            {
                // Check if request tries to change restricted fields
                if (exam.SectionId != request.SectionId ||
                    exam.AvailableFrom != request.AvailableFrom ||
                    exam.AvailableTo != request.AvailableTo ||
                    exam.DurationMinutes != request.DurationMinutes ||
                    exam.AttemptsAllowed != request.AttemptsAllowed ||
                    exam.ShuffleQuestions != request.ShuffleQuestions ||
                    exam.WeekNumber != request.WeekNumber)
                {
                    return (0, new ErrorResponse { Errors = ["Published exams can only have their Title updated. Other fields are locked."] });
                }

                // Only update Title
                exam.Title = request.Title;
            }
            else
            {
                // For Draft exams, apply all fields
                request.Adapt(exam);
            }

            // Preserve system fields
            exam.UpdatedAt = DateTime.UtcNow;

            var rows = await _examRepository.UpdateAsync(exam);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            // Block deletion of published or closed exams
            if (exam.ExamStatus == ExamStatus.Published || exam.ExamStatus == ExamStatus.Closed)
                return (0, new ErrorResponse { Errors = ["Cannot delete a published or closed exam."] });

            // TODO: Later add check for dependent entities (ExamQuestions/Attempts)
            // If exam has questions or attempts, block deletion

            var rows = await _examRepository.DeleteAsync(exam);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> ChangeExamStatusAsync(int id, ExamStatus newStatus)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            // Validate status transitions
            bool isValidTransition = (exam.ExamStatus == ExamStatus.Draft && newStatus == ExamStatus.Published) ||
                                     (exam.ExamStatus == ExamStatus.Published && newStatus == ExamStatus.Closed);

            if (!isValidTransition)
            {
                return (false, new ErrorResponse
                {
                    Errors = [$"Invalid status transition from {exam.ExamStatus} to {newStatus}. " +
                              "Valid transitions: Draft -> Published, Published -> Closed."]
                });
            }

            // On publish, validate exam is ready
            if (newStatus == ExamStatus.Published)
            {
                if (string.IsNullOrWhiteSpace(exam.Title))
                    return (false, new ErrorResponse { Errors = ["Exam must have a title to be published."] });

                // Ensure exam has at least 1 question
                var examQuestions = await _examQuestionRepository.GetAllDetailsByExamIdAsync(exam.Id);
                if (!examQuestions.Any())
                    return (false, new ErrorResponse { Errors = ["Exam must have at least one question to be published."] });

                // Ensure all MCQ questions have at least 1 choice marked as IsCorrect
                var mcqQuestionIds = examQuestions
                    .Where(eq => eq.Question.Type == "MultipleChoiceQuestion")
                    .Select(eq => eq.QuestionId)
                    .Distinct()
                    .ToList();

                if (mcqQuestionIds.Any())
                {
                    var allChoices = await _choiceRepository.GetByQuestionIdsAsync(mcqQuestionIds);
                    var choicesByQuestionId = allChoices.GroupBy(c => c.QuestionId)
                        .ToDictionary(g => g.Key, g => g.ToList());

                    foreach (var mcqId in mcqQuestionIds)
                    {
                        if (!choicesByQuestionId.ContainsKey(mcqId))
                        {
                            return (false, new ErrorResponse 
                            { 
                                Errors = [$"Multiple choice question (ID: {mcqId}) has no choices defined."] 
                            });
                        }

                        var choices = choicesByQuestionId[mcqId];
                        if (!choices.Any(c => c.IsCorrect))
                        {
                            return (false, new ErrorResponse 
                            { 
                                Errors = [$"Multiple choice question (ID: {mcqId}) must have at least one correct answer."] 
                            });
                        }
                    }
                }
            }

            exam.ExamStatus = newStatus;
            exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateAsync(exam);

            return (true, new SuccessResponse<string> { Data = $"Exam status changed to {newStatus} successfully." });
        }
    }
}
