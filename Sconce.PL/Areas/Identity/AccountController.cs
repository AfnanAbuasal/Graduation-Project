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

        [HttpPost("Login")]
        public async Task<ActionResult<Response>> Login([FromBody] LoginRequest loginRequest)
        {
            var result = await _authenticationService.LoginAsync(loginRequest);
            return Ok(result);
        }

        [HttpGet("ConfirmEmail")]
        public async Task<ActionResult<Response>> ConfirmEmail([FromQuery] string token, [FromQuery] string userID)
        {
            var result = await _authenticationService.ConfirmEmailAsync(token, userID);
            return Ok(result);
        }

        [HttpPost("ForgotPassword")]
        public async Task<ActionResult<Response>> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            var result = await _authenticationService.ForgotPasswordAsync(forgotPasswordRequest);
            return Ok(result);
        }

        [HttpPatch("ResetPassword")]
        public async Task<ActionResult<Response>> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            var result = await _authenticationService.ResetPasswordAsync(resetPasswordRequest);
            return Ok(result);
        }
    }
}
