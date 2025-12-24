using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using System.Security.Claims;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class QuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // Gets all questions (mixed types) for a specific course.
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Response>> GetByCourseId([FromRoute] int courseId)
        {
            var result = await _questionService.GetAllByCourseIdAsync(courseId);
            return Ok(result);
        }

        // Gets all questions created by the current instructor.
        [HttpGet("Mine")]
        public async Task<ActionResult<Response>> GetMine()
        {
            var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(instructorId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var result = await _questionService.GetAllByInstructorIdAsync(instructorId);
            return Ok(result);
        }

        // Gets all questions by type.
        [HttpGet("Type/{type}")]
        public async Task<ActionResult<Response>> GetByType([FromRoute] QuestionType type)
        {
            var result = await _questionService.GetAllByTypeAsync(type);
            return Ok(result);
        }

        // Gets all questions by difficulty.
        [HttpGet("Difficulty/{difficulty}")]
        public async Task<ActionResult<Response>> GetByDifficulty([FromRoute] Difficulty difficulty)
        {
            var result = await _questionService.GetAllByDifficultyAsync(difficulty);
            return Ok(result);
        }

        // Searches questions by prompt text within a course.
        [HttpGet("Course/{courseId}/Search")]
        public async Task<ActionResult<Response>> Search([FromRoute] int courseId, [FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
                return BadRequest(new ErrorResponse { Errors = ["Search term is required."] });

            var result = await _questionService.SearchByPromptAsync(courseId, term);
            return Ok(result);
        }

        // Gets question count for a course.
        [HttpGet("Course/{courseId}/Count")]
        public async Task<ActionResult<Response>> CountByCourse([FromRoute] int courseId)
        {
            var result = await _questionService.CountByCourseAsync(courseId);
            return Ok(result);
        }

        // Gets question count by type for a course.
        [HttpGet("Course/{courseId}/Count/Type/{type}")]
        public async Task<ActionResult<Response>> CountByType([FromRoute] int courseId, [FromRoute] QuestionType type)
        {
            var result = await _questionService.CountByTypeAsync(courseId, type);
            return Ok(result);
        }
    }
}
