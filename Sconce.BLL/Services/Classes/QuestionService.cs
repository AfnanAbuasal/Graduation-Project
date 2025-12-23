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

namespace Sconce.BLL.Services.Classes
{
    public class QuestionService : GenericService<QuestionRequest, QuestionResponse, Question>, IQuestionService
    {
        private readonly IQuestionRepository _questionRepository;
        private readonly ICourseRepository _courseRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public QuestionService(IQuestionRepository questionRepository, ICourseRepository courseRepository, IHttpContextAccessor httpContextAccessor) : base(questionRepository)
        {
            _questionRepository = questionRepository;
            _courseRepository = courseRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(QuestionRequest request)
        {
            // Ensure course exists before creating question
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            var question = request.Adapt<Question>();
            question.CreatedByInstructorId = instructorId;
            var rows = await _questionRepository.AddAsync(question);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, QuestionRequest request)
        {
            var entity = await _questionRepository.GetByIdAsync(ID);
            if (entity == null)
                return (0, new ErrorResponse { Errors = ["Not Found."] });

            var instructorId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return (0, new ErrorResponse { Errors = ["User not authenticated."] });

            if (entity.CreatedByInstructorId != instructorId)
                return (0, new ErrorResponse { Errors = ["Not authorized to update this question."] });

            // Ensure course exists
            var course = await _courseRepository.GetByIdAsync(request.CourseId);
            if (course == null)
                return (0, new ErrorResponse { Errors = [$"Course with Id: {request.CourseId} not found."] });

            request.Adapt(entity);
            entity.UpdatedAt = DateTime.UtcNow;

            var rows = await _questionRepository.UpdateAsync(entity);
            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
        }

        public async Task<Response> GetAllByCourseIdAsync(int courseId)
        {
            var questions = await _questionRepository.GetAllByCourseIdAsync(courseId);
            var responseList = questions.Adapt<IEnumerable<QuestionResponse>>();
            return new SuccessResponse<IEnumerable<QuestionResponse>> { Data = responseList };
        }

        public async Task<Response> GetAllByInstructorIdAsync(string instructorId)
        {
            var questions = await _questionRepository.GetByCreatedByInstructorIdAsync(instructorId);
            var responseList = questions.Adapt<IEnumerable<QuestionResponse>>();
            return new SuccessResponse<IEnumerable<QuestionResponse>> { Data = responseList };
        }
    }
}
