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
    public class StudentController : ControllerBase
    {
        private readonly IAdminStudentService _adminStudentService;

        public StudentController(IAdminStudentService adminStudentService)
        {
            _adminStudentService = adminStudentService;
        }

        [HttpGet("Applications")]
        public async Task<IActionResult> GetAllApplications([FromQuery] ApplicationStatus? status = null)
        {
            var result = await _adminStudentService.GetAllApplicationsAsync(status);
            return Ok(result);
        }

        [HttpGet("Applications/{id}")]
        public async Task<ActionResult<Response>> GetApplicationById([FromRoute] int id)
        {
            var result = await _adminStudentService.GetApplicationByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPost("Applications/{id}/Review")]
        public async Task<ActionResult<Response>> ReviewApplication([FromRoute] int id, [FromBody] ApplicationReviewRequest request)
        {
            var result = await _adminStudentService.ReviewApplicationAsync(id, request.ApplicationStatus, request.Feedback);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
