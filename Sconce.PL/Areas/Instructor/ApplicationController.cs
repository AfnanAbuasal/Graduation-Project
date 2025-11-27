using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

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
        public async Task<ActionResult<Response>> Apply([FromForm] InstructorApplicationRequest request)
        {
            var result = await _applicationService.SubmitApplicationAsync(request);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpGet("Status")]
        public async Task<ActionResult<Response>> GetStatus([FromQuery] string email)
        {
            var result = await _applicationService.GetApplicationByEmailAsync(email);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
