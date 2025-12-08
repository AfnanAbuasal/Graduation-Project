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

        [HttpGet]
        public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
        {
            var requests = await _dropoutService.GetAllAsync(onlyActive);
            return Ok(requests);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _dropoutService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

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
