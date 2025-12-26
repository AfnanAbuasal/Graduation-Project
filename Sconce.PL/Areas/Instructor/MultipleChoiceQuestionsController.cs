using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class MultipleChoiceQuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public MultipleChoiceQuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // Creates a new multiple choice question.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] MultipleChoiceQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _questionService.CreateMultipleChoiceQuestionAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing multiple choice question.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] MultipleChoiceQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _questionService.UpdateMultipleChoiceQuestionAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets a specific multiple choice question by ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _questionService.GetMultipleChoiceByIdAsync(id);
            if (!result.Success) return NotFound(result.Response);
            return Ok(result.Response);
        }

        // Gets all multiple choice questions for a specific course.
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Response>> GetByCourseId([FromRoute] int courseId)
        {
            var result = await _questionService.GetAllMultipleChoiceByCourseIdAsync(courseId);
            return Ok(result);
        }

        // Deletes a multiple choice question.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _questionService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
