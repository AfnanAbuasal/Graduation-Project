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
    public class StudentController : ControllerBase
    {
        private readonly IAdminStudentService _adminStudentService;

        public StudentController(IAdminStudentService adminStudentService)
        {
            _adminStudentService = adminStudentService;
        }

        [HttpGet("applications")]
        public async Task<IActionResult> GetAllApplications([FromQuery] ApplicationStatus? status = null)
        {
            var result = await _adminStudentService.GetAllApplicationsAsync(status);
            return Ok(result);
        }

        [HttpGet("applications/{id}")]
        public async Task<IActionResult> GetApplicationById(int id)
        {
            var result = await _adminStudentService.GetApplicationByIdAsync(id);
            if (result == null) return NotFound("Application not found.");
            return Ok(result);
        }

        [HttpPost("applications/{id}/review")]
        public async Task<IActionResult> ReviewApplication(int id, [FromBody] ApplicationReviewRequest request)
        {
            var success = await _adminStudentService.ReviewApplicationAsync(id, request.ApplicationStatus, request.Feedback);
            if (!success) return BadRequest("Failed to review the application.");
            return Ok("Application reviewed successfully.");
        }
    }
}
