using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;

namespace Sconce.PL.Areas.Admin
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Admin")]
    [Authorize(Roles = "Admin,Super Admin")]
    public class InstructorController : ControllerBase
    {
        private readonly IAdminInstructorService _adminInstructorService;
        public InstructorController(IAdminInstructorService adminInstructorService)
        {
            _adminInstructorService = adminInstructorService;
        }

        [HttpGet("Applications")]
        public async Task<IActionResult> GetAll([FromQuery] ApplicationStatus? status)
        {
            var result = await _adminInstructorService.GetAllApplicationsAsync(status);
            return Ok(result);
        }

        [HttpGet("Applications/{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _adminInstructorService.GetApplicationByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPost("Applications/{id}/Review")]
        public async Task<ActionResult<Response>> Review([FromRoute] int id, [FromBody] ApplicationReviewRequest request)
        {
            var result = await _adminInstructorService.ReviewApplicationAsync(id, request.ApplicationStatus, request.Feedback);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
