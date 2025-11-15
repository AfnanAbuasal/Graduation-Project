using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;

namespace Sconce.PL.Areas.Instructor
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Instructor")]
    public class ApplicationController : ControllerBase
    {
        private readonly IInstructorApplicationService _applicationService;

        public ApplicationController(IInstructorApplicationService applicationService)
        {
            _applicationService = applicationService;
        }

        [HttpPost("Apply")]
        public async Task<IActionResult> Apply([FromForm] InstructorApplicationRequest request)
        {
            var response = await _applicationService.SubmitApplicationAsync(request);
            return Ok(response);
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetStatus([FromQuery] string email)
        {
            var response = await _applicationService.GetApplicationByEmailAsync(email);
            if (response == null) return NotFound("No application found for this email.");
            return Ok(response);
        }
    }
}
