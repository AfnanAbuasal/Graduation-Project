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
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Models.Enums;

namespace Sconce.BLL.Services.Classes
{
    public class QuestionService : GenericService<QuestionRequest, QuestionResponse, Question>, IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IFileService _fileService;
        private readonly IExamQuestionRepository _examQuestionRepository;

        public QuestionService(IQuestionRepository questionRepository, ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor, IFileService fileService, IExamQuestionRepository examQuestionRepository) : base(questionRepository)
        {
            _questionRepository = questionRepository;
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
            _fileService = fileService;
            _examQuestionRepository = examQuestionRepository;
        }

        public async Task<Response> GetAllByCourseIdAsync(int courseId)
        {
            var questions = await _questionRepository.GetAllByCourseIdAsync(courseId);
            var responseList = questions.Select(q =>
            {
                return q switch
                {
                    MultipleChoiceQuestion mcq => (object)mcq.Adapt<MultipleChoiceQuestionResponse>(),
                    EssayQuestion eq => (object)eq.Adapt<EssayQuestionResponse>(),
                    _ => (object)q.Adapt<QuestionResponse>()
                };
            }).ToList();
            return new SuccessResponse<IEnumerable<object>> { Data = responseList };
        }

        public async Task<Response> GetAllByInstructorIdAsync(string instructorId)
        {
            var questions = await _questionRepository.GetByCreatedByInstructorIdAsync(instructorId);
            var responseList = questions.Select(q =>
            {
                return q switch
                {
                    MultipleChoiceQuestion mcq => (object)mcq.Adapt<MultipleChoiceQuestionResponse>(),
                    EssayQuestion eq => (object)eq.Adapt<EssayQuestionResponse>(),
                    _ => (object)q.Adapt<QuestionResponse>()
                };
            }).ToList();
            return new SuccessResponse<IEnumerable<object>> { Data = responseList };
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(QuestionRequest request)
        {
            // This method should not be used directly - use CreateMultipleChoiceQuestionAsync or CreateEssayQuestionAsync
            return (0, new ErrorResponse { Errors = ["Use CreateMultipleChoiceQuestionAsync or CreateEssayQuestionAsync instead."] });
        }

        public async Task<(int NumberOfEntries, Response Response)> CreateMultipleChoiceQuestionAsync(MultipleChoiceQuestionRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            var question = request.Adapt<MultipleChoiceQuestion>();
            question.CreatedByInstructorId = instructorId;
            var rows = await _questionRepository.AddAsync(question);
            return (rows, new SuccessResponse<MultipleChoiceQuestionResponse> { Data = question.Adapt<MultipleChoiceQuestionResponse>() });
        }

        public async Task<(int NumberOfEntries, Response Response)> CreateEssayQuestionAsync(EssayQuestionRequest request)
        {
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            var question = request.Adapt<EssayQuestion>();
            question.CreatedByInstructorId = instructorId;

            // Handle file upload if provided
            if (request.File != null)
            {
                try
                {
                    question.QuestionFilePath = await _fileService.SaveFileAsync(request.File, "Uploads/Questions");
                }
                catch (Exception ex)
                {
                    return (0, new ErrorResponse { Errors = [$"File upload failed: {ex.Message}"] });
                }
            }

            var rows = await _questionRepository.AddAsync(question);
            return (rows, new SuccessResponse<EssayQuestionResponse> { Data = question.Adapt<EssayQuestionResponse>() });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, QuestionRequest request)
        {
            // This method should not be used directly - use UpdateMultipleChoiceQuestionAsync or UpdateEssayQuestionAsync
            return (0, new ErrorResponse { Errors = ["Use UpdateMultipleChoiceQuestionAsync or UpdateEssayQuestionAsync instead."] });
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateMultipleChoiceQuestionAsync(int id, MultipleChoiceQuestionRequest request)
        {
            var entity = await _questionRepository.GetMultipleChoiceByIdAsync(id);
            if (entity == null)
                return (0, new ErrorResponse { Errors = ["Question not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            if (entity.CreatedByInstructorId != instructorId)
                return (0, new ErrorResponse { Errors = ["Not authorized to update this question."] });

            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            request.Adapt(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            var rows = await _questionRepository.UpdateAsync(entity);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateEssayQuestionAsync(int id, EssayQuestionRequest request)
        {
            var entity = await _questionRepository.GetEssayByIdAsync(id);
            if (entity == null)
                return (0, new ErrorResponse { Errors = ["Question not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            if (entity.CreatedByInstructorId != instructorId)
                return (0, new ErrorResponse { Errors = ["Not authorized to update this question."] });

            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            // Handle file upload if a new file is provided
            if (request.File != null)
            {
                try
                {
                    // Delete old file if exists
                    if (!string.IsNullOrEmpty(entity.QuestionFilePath))
                    {
                        await _fileService.DeleteFileAsync(entity.QuestionFilePath);
                    }

                    // Save new file
                    entity.QuestionFilePath = await _fileService.SaveFileAsync(request.File, "Uploads/Questions");
                }
                catch (Exception ex)
                {
                    return (0, new ErrorResponse { Errors = [$"File upload failed: {ex.Message}"] });
                }
            }

            request.Adapt(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            var rows = await _questionRepository.UpdateAsync(entity);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
        {
            var entity = await _questionRepository.GetByIdAsync(Id);
            if (entity == null)
                return (0, new ErrorResponse { Errors = ["Question not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            if (entity.CreatedByInstructorId != instructorId)
                return (0, new ErrorResponse { Errors = ["Not authorized to delete this question."] });

            var linkedToExam = await _examQuestionRepository.ExistsQuestionInAnyExamAsync(Id);
            if (linkedToExam)
                return (0, new ErrorResponse { Errors = ["Cannot delete question because it is linked to an exam."] });

            // Delete associated file if it's an essay question with a file
            if (entity is EssayQuestion essayQuestion && !string.IsNullOrEmpty(essayQuestion.QuestionFilePath))
                await _fileService.DeleteFileAsync(essayQuestion.QuestionFilePath);
            
            var rows = await _questionRepository.DeleteAsync(entity);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> GetMultipleChoiceByIdAsync(int id)
        {
            var question = await _questionRepository.GetMultipleChoiceByIdAsync(id);
            if (question == null)
                return (false, new ErrorResponse { Errors = ["Question not found."] });

            var response = question.Adapt<MultipleChoiceQuestionResponse>();
            return (true, new SuccessResponse<MultipleChoiceQuestionResponse> { Data = response });
        }

        public async Task<(bool Success, Response Response)> GetEssayByIdAsync(int id)
        {
            var question = await _questionRepository.GetEssayByIdAsync(id);
            if (question == null)
                return (false, new ErrorResponse { Errors = ["Question not found."] });

            var response = question.Adapt<EssayQuestionResponse>();
            return (true, new SuccessResponse<EssayQuestionResponse> { Data = response });
        }

        public async Task<Response> GetAllMultipleChoiceByCourseIdAsync(int courseId)
        {
            var mcqs = await _questionRepository.GetAllMultipleChoiceByCourseIdAsync(courseId);
            var data = mcqs.Adapt<IEnumerable<MultipleChoiceQuestionResponse>>();
            return new SuccessResponse<IEnumerable<MultipleChoiceQuestionResponse>> { Data = data };
        }

        public async Task<Response> GetMultipleChoiceByCourseAndSelectionModeAsync(int courseId, bool allowMultiple)
        {
            var mcqs = await _questionRepository.GetMultipleChoiceByCourseAndSelectionModeAsync(courseId, allowMultiple);
            var data = mcqs.Adapt<IEnumerable<MultipleChoiceQuestionResponse>>();
            return new SuccessResponse<IEnumerable<MultipleChoiceQuestionResponse>> { Data = data };
        }

        public async Task<Response> GetAllEssayByCourseIdAsync(int courseId)
        {
            var essays = await _questionRepository.GetAllEssayByCourseIdAsync(courseId);
            var data = essays.Adapt<IEnumerable<EssayQuestionResponse>>();
            return new SuccessResponse<IEnumerable<EssayQuestionResponse>> { Data = data };
        }

        public async Task<Response> GetAllByDifficultyAndCourseAsync(int courseId, Difficulty difficulty)
        {
            var questions = await _questionRepository.GetAllByDifficultyAndCourseAsync(courseId, difficulty);
            var responseList = questions.Select(q =>
            {
                return q switch
                {
                    MultipleChoiceQuestion mcq => (object)mcq.Adapt<MultipleChoiceQuestionResponse>(),
                    EssayQuestion eq => (object)eq.Adapt<EssayQuestionResponse>(),
                    _ => (object)q.Adapt<QuestionResponse>()
                };
            }).ToList();
            return new SuccessResponse<IEnumerable<object>> { Data = responseList };
        }

        public async Task<Response> SearchByPromptAsync(int courseId, string term)
        {
            var questions = await _questionRepository.SearchByPromptAsync(courseId, term);
            var responseList = questions.Select(q =>
            {
                return q switch
                {
                    MultipleChoiceQuestion mcq => (object)mcq.Adapt<MultipleChoiceQuestionResponse>(),
                    EssayQuestion eq => (object)eq.Adapt<EssayQuestionResponse>(),
                    _ => (object)q.Adapt<QuestionResponse>()
                };
            }).ToList();
            return new SuccessResponse<IEnumerable<object>> { Data = responseList };
        }

        public async Task<Response> CountByTypeAsync(int courseId, string type)
        {
            var count = await _questionRepository.CountByTypeAsync(courseId, type);
            return new SuccessResponse<int> { Data = count };
        }
        public async Task<Response> CountByCourseAsync(int courseId)
        {
            var count = await _questionRepository.CountByCourseAsync(courseId);
            return new SuccessResponse<int> { Data = count };
        }
    }
}
