using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    public class ApplicationController : ControllerBase
    {
        private readonly IStudentApplicationService _applicationService;

        public ApplicationController(IStudentApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost("apply")]
        public async Task<IActionResult> Apply([FromForm] StudentApplicationRequest request)
        {
            var response = await _applicationService.SubmitApplicationAsync(request);
            return Ok(response);
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus([FromQuery] string email)
        {
            var response = await _applicationService.GetApplicationByEmailAsync(email);
            if (response == null) return NotFound("No application found for this email.");
            return Ok(response);
        }
    }
}
