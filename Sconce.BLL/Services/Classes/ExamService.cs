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
        private readonly IProgramRepository _programRepository;

        public ExamService(
            IExamRepository examRepository,
            ISectionRepository sectionRepository,
            IExamQuestionRepository examQuestionRepository,
            IChoiceRepository choiceRepository,
            IHttpContextAccessor httpContextAccessor,
            IProgramRepository programRepository)
            : base(examRepository)
        {
            _examRepository = examRepository;
            _sectionRepository = sectionRepository;
            _examQuestionRepository = examQuestionRepository;
            _choiceRepository = choiceRepository;
            _httpContextAccessor = httpContextAccessor;
            _programRepository = programRepository;
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
            // Validate exactly one of SectionId or ProgramId is set
            bool hasSectionId = request.SectionId.HasValue;
            bool hasProgramId = request.ProgramId.HasValue;

            if ((hasSectionId && hasProgramId) || (!hasSectionId && !hasProgramId))
                return (0, new ErrorResponse { Errors = ["Exam must belong to either a Section (normal flow) or a Program (proficiency flow), but not both or neither."] });

            // Validate Section exists if SectionId is provided
            if (hasSectionId)
            {
                var section = await _sectionRepository.GetByIdAsync(request.SectionId!.Value);
                if (section == null)
                    return (0, new ErrorResponse { Errors = ["Section not found."] });
            }

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
            
            // Update section timestamp if exam belongs to a section
            if (rows > 0 && request.SectionId.HasValue)
            {
                await UpdateSectionTimestampAsync(request.SectionId.Value);
            }
            
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

        public async Task<Response> GetAllByProgramAsync(int programId, string instructorId, bool onlyActive = false)
        {
            // Validate Program exists
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return new ErrorResponse { Errors = ["Program not found."] };

            // Verify program is assigned to the instructor as exam writer
            if (program.ExamWriterInstructorId != instructorId)
                return new ErrorResponse { Errors = ["Unauthorized access to this program."] };

            // Get all exams for this program (proficiency flow)
            var exams = await _examRepository.GetAllByProgramIdAsync(programId, withTracking: false);

            if (onlyActive)
                exams = exams.Where(e => e.Status == Status.Active);

            var examResponses = exams.Adapt<IEnumerable<ExamResponse>>().ToList();

            return new SuccessResponse<IEnumerable<ExamResponse>> { Data = examResponses };
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ExamRequest request)
        {
            // Fetch existing exam
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            var originalSectionId = exam.SectionId;

            // Block updates if exam is closed
            if (exam.ExamStatus == ExamStatus.Closed)
                return (0, new ErrorResponse { Errors = ["Closed exams cannot be edited."] });

            // Validate exactly one of SectionId or ProgramId is set
            bool hasSectionId = request.SectionId.HasValue;
            bool hasProgramId = request.ProgramId.HasValue;

            if ((hasSectionId && hasProgramId) || (!hasSectionId && !hasProgramId))
                return (0, new ErrorResponse { Errors = ["Exam must belong to either a Section (normal flow) or a Program (proficiency flow), but not both or neither."] });

            // Validate Section exists if SectionId is provided
            if (hasSectionId)
            {
                var section = await _sectionRepository.GetByIdAsync(request.SectionId!.Value);
                if (section == null)
                    return (0, new ErrorResponse { Errors = ["Section not found."] });
            }

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
            
            // Update section timestamp if exam belongs to a section (original or new)
            if (rows > 0)
            {
                if (originalSectionId.HasValue)
                {
                    await UpdateSectionTimestampAsync(originalSectionId.Value);
                }
                if (request.SectionId.HasValue && request.SectionId != originalSectionId)
                {
                    await UpdateSectionTimestampAsync(request.SectionId.Value);
                }
            }
            
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (0, new ErrorResponse { Errors = ["Exam not found."] });

            var sectionId = exam.SectionId;

            // Block deletion of published or closed exams
            if (exam.ExamStatus == ExamStatus.Published || exam.ExamStatus == ExamStatus.Closed)
                return (0, new ErrorResponse { Errors = ["Cannot delete a published or closed exam."] });

            // Check for dependent entities (ExamQuestions/Attempts)
            var examQuestions = await _examQuestionRepository.GetAllByExamIdAsync(id);
            if (examQuestions.Any())
                return (0, new ErrorResponse { Errors = ["Cannot delete an exam that has questions linked to it."] });

            var rows = await _examRepository.DeleteAsync(exam);
            
            // Update section timestamp if exam belonged to a section
            if (rows > 0 && sectionId.HasValue)
            {
                await UpdateSectionTimestampAsync(sectionId.Value);
            }
            
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

            // Update section timestamp if exam belongs to a section
            if (exam.SectionId.HasValue)
            {
                await UpdateSectionTimestampAsync(exam.SectionId.Value);
            }

            return (true, new SuccessResponse<string> { Data = $"Exam status changed to {newStatus} successfully." });
        }

        public async Task<(bool Success, Response Response)> GetExamStatusAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            var statusResponse = new ExamStatusResponse { Id = exam.Id, ExamStatus = exam.ExamStatus };
            return (true, new SuccessResponse<ExamStatusResponse> { Data = statusResponse });
        }
        public async Task<(bool Success, Response Response)> ReopenProficiencyExamAsync(int id)
        {
            // Extract instructor ID from claims
            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (false, new ErrorResponse { Errors = ["User not authenticated."] });

            // Fetch the exam
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null)
                return (false, new ErrorResponse { Errors = ["Exam not found."] });

            // Validate it's a proficiency exam (has ProgramId, not SectionId)
            if (!exam.ProgramId.HasValue || exam.SectionId.HasValue)
                return (false, new ErrorResponse { Errors = ["Only proficiency exams can be re-opened. Regular section exams cannot be re-opened once published or closed."] });

            // Validate current status is Published or Closed
            if (exam.ExamStatus != ExamStatus.Published && exam.ExamStatus != ExamStatus.Closed)
                return (false, new ErrorResponse { Errors = ["Only Published or Closed exams can be re-opened. Current status: " + exam.ExamStatus] });

            // Validate instructor is the exam writer for this program
            var program = await _programRepository.GetByIdAsync(exam.ProgramId.Value);
            if (program == null)
                return (false, new ErrorResponse { Errors = ["Associated program not found."] });

            if (program.ExamWriterInstructorId != instructorId)
                return (false, new ErrorResponse { Errors = ["Forbidden. You are not assigned as the exam writer for this program."] });

            // Re-open the exam by setting status back to Draft
            exam.ExamStatus = ExamStatus.Draft;
            exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateAsync(exam);

            return (true, new SuccessResponse<string> { Data = "Proficiency exam re-opened for editing successfully. Status changed to Draft." });
        }
        private async Task UpdateSectionTimestampAsync(int sectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(sectionId);
            if (section != null)
            {
                section.UpdatedAt = DateTime.UtcNow;
                await _sectionRepository.UpdateAsync(section);
            }
        }
    }
}
