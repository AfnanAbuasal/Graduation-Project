using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class DropoutController : ControllerBase
    {
        private readonly IDropoutService _dropoutService;

        public DropoutController(IDropoutService dropoutService)
        {
            _dropoutService = dropoutService;
        }

        // Lists all dropout requests, optionally only the active ones.
        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var requests = await _dropoutService.GetAllAsync(onlyActive);
            return Ok(requests);
        }

        // Shows details for a specific dropout request.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _dropoutService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Reviews and updates the status of a dropout request.
        [HttpPatch("{id}/Review")]
        public async Task<ActionResult<Response>> ReviewRequest([FromRoute] int id, [FromBody] ApplicationReviewRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _dropoutService.ReviewDropoutAsync(id, request.ApplicationStatus, request.Feedback);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
