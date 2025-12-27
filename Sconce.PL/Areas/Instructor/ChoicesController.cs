using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    [Authorize(Roles = "Instructor")]
    public class ChoicesController : ControllerBase
    {
        private readonly IChoiceService _choiceService;

        public ChoicesController(IChoiceService choiceService)
        {
            _choiceService = choiceService;
        }

        // Creates a new choice for a multiple choice question.
        [HttpPost("{questionId}")]
        public async Task<ActionResult<Response>> Create([FromRoute] int questionId, [FromBody] ChoiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _choiceService.CreateAsync(questionId, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets all choices for a specific multiple choice question.
        [HttpGet("{questionId}")]
        public async Task<ActionResult<Response>> GetByQuestionId([FromRoute] int questionId)
        {
            var result = await _choiceService.GetByQuestionIdAsync(questionId);
            return Ok(result);
        }

        // Updates an existing choice.
        [HttpPut("{choiceId}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int choiceId, [FromBody] ChoiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _choiceService.UpdateAsync(choiceId, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes a choice.
        [HttpDelete("{choiceId}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int choiceId)
        {
            var result = await _choiceService.DeleteAsync(choiceId);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
