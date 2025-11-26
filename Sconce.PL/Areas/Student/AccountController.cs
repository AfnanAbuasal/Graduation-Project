using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("RegisterStudent")]
        public async Task<ActionResult<Response>> RegisterStudent(StudentRegisterRequest registerRequest)
        {
            var result = await _authenticationService.RegisterStudentAsync(registerRequest);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }

        [HttpGet("ApproveParentLink")]
        public async Task<IActionResult> ApproveParentLink([FromQuery] string token)
        {
            var result = await _authenticationService.ApproveParentLinkAsync(token);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}