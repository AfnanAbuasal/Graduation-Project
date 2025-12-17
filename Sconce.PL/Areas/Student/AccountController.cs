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

        // Registers a new student account.
        [HttpPost("RegisterStudent")]
        public async Task<ActionResult<Response>> RegisterStudent(StudentRegisterRequest registerRequest)
        {
            var result = await _authenticationService.RegisterStudentAsync(registerRequest);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Approves a parent link using a verification token.
        [HttpGet("ApproveParentLink")]
        public async Task<ActionResult<Response>> ApproveParentLink([FromQuery] string token)
        {
            var result = await _authenticationService.ApproveParentLinkAsync(token);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}