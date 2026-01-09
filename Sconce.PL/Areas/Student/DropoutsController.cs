using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class DropoutsController : ControllerBase
    {
        private readonly IDropoutService _dropoutService;

        public DropoutsController(IDropoutService dropoutService)
        {
            _dropoutService = dropoutService;
        }

        // Gets the current student's dropout request for a program.
        [HttpGet("Program/{id}")]
        public async Task<ActionResult<Response>> GetByProgram([FromRoute] int id)
        {
            var result = await _dropoutService.GetStudentDropoutByProgramIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Submits a new dropout request.
        [HttpPost]
        public async Task<ActionResult<Response>> RequestDropout([FromBody] DropoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _dropoutService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Updates an existing dropout request.
        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> UpdateRequest([FromRoute] int id, [FromBody] DropoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _dropoutService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Cancels a dropout request.
        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> CancelRequest([FromRoute] int id)
        {
            var result = await _dropoutService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
