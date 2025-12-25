using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[controller]")]
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
        [HttpPost]
        public async Task<ActionResult<Response>> Create([FromBody] ChoiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _choiceService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing choice.
        [HttpPut("{questionId}/{text}")]
        public async Task<ActionResult<Response>> Update([FromRoute] int questionId, [FromRoute] string text, [FromBody] ChoiceRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _choiceService.UpdateAsync(questionId, text, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Deletes a choice.
        [HttpDelete("{questionId}/{text}")]
        public async Task<ActionResult<Response>> Delete([FromRoute] int questionId, [FromRoute] string text)
        {
            var result = await _choiceService.DeleteAsync(questionId, text);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Gets a specific choice by question ID and text (composite key).
        //[HttpGet("{questionId}/{text}")]
        //public async Task<ActionResult<Response>> GetById([FromRoute] int questionId, [FromRoute] string text)
        //{
        //    var result = await _choiceService.GetByIdAsync(questionId, text);
        //    if (!result.Success) return NotFound(result.Response);
        //    return Ok(result.Response);
        //}

        // Gets all choices for a specific multiple choice question.
        [HttpGet("Question/{questionId}")]
        public async Task<ActionResult<Response>> GetByQuestionId([FromRoute] int questionId)
        {
            var result = await _choiceService.GetByQuestionIdAsync(questionId);
            return Ok(result);
        }
    }
}
