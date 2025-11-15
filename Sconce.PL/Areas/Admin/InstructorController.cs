using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
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
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            var result = await _adminInstructorService.GetApplicationByIdAsync(id);
            if (result == null) return NotFound("Application not found.");
            return Ok(result);
        }

        [HttpPost("Applications/{id}/Review")]
        public async Task<IActionResult> Review([FromRoute] int id, [FromBody] ApplicationReviewRequest request)
        {
            var success = await _adminInstructorService.ReviewApplicationAsync(id, request.ApplicationStatus, request.Feedback);
            if (!success) return BadRequest("Failed to review the application.");
            return Ok("Application reviewed successfully.");
        }
    }
}
