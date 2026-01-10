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
    public class EssayQuestionsController : ControllerBase
    {
        private readonly IQuestionService _questionService;

        public EssayQuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
        }

        // Creates a new essay question.
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromForm] EssayQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _questionService.CreateEssayQuestionAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing essay question.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromForm] EssayQuestionRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _questionService.UpdateEssayQuestionAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets a specific essay question by ID.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _questionService.GetEssayByIdAsync(id);
            if (!result.Success) return NotFound(result.Response);
            return Ok(result.Response);
        }

        // Gets all essay questions for a specific course.
        [HttpGet("Course/{courseId}")]
        public async Task<ActionResult<Response>> GetByCourseId([FromRoute] int courseId)
        {
            var result = await _questionService.GetAllEssayByCourseIdAsync(courseId);
            return Ok(result);
        }

        // Gets all essay questions for a specific program (proficiency flow).
        [HttpGet("Program/{programId}")]
        public async Task<ActionResult<Response>> GetByProgramId([FromRoute] int programId)
        {
            var result = await _questionService.GetAllEssayByProgramIdAsync(programId);
            return Ok(result);
        }

        // Deletes an essay question.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int id)
        {
            var result = await _questionService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
