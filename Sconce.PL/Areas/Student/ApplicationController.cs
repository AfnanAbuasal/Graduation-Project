using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

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

        // Submits a new student application.
        [HttpPost("Apply")]
        public async Task<ActionResult<Response>> Apply([FromForm] StudentApplicationRequest request)
        {
            var result = await _applicationService.SubmitApplicationAsync(request);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Retrieves the application status for a student by email.
        [HttpGet("Status")]
        public async Task<ActionResult<Response>> GetStatus([FromQuery] string email)
        {
            var result = await _applicationService.GetApplicationByEmailAsync(email);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
