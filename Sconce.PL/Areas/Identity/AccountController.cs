using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sconce.PL.Areas.Identity
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Identity")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        // Authenticates a user with their credentials.
        [HttpPost("Login")]
        public async Task<ActionResult<Response>> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _authenticationService.LoginAsync(loginRequest);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Confirms a user's email using the verification token and user identifier.
        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<Response>> ConfirmEmail([FromQuery] string token, [FromQuery] string userID)
        {
            var result = await _authenticationService.ConfirmEmailAsync(token, userID);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Starts the password reset flow by sending a reset email.
        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<Response>> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            var result = await _authenticationService.ForgotPasswordAsync(forgotPasswordRequest);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Resets a user's password using the provided token and new password.
        [HttpPatch("ResetPassword")]
        public async Task<ActionResult<Response>> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetPasswordRequest);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}
