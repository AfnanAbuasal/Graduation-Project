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
    public class DropoutController : ControllerBase
    {
        private readonly IDropoutService _dropoutService;

        public DropoutController(IDropoutService dropoutService)
        {
            _dropoutService = dropoutService;
        }

        [HttpPost]
        public async Task<ActionResult<Response>> RequestDropout([FromBody] DropoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _dropoutService.CreateAsync(request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Response>> UpdateRequest([FromRoute] int id, [FromBody] DropoutRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _dropoutService.UpdateAsync(id, request);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Response>> CancelRequest([FromRoute] int id)
        {
            var result = await _dropoutService.DeleteAsync(id);
            if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
