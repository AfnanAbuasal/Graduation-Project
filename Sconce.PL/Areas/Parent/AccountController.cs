using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Sconce.PL.Areas.Parent
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Parent")]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;

        public AccountController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost("RegisterParent")]
        public async Task<ActionResult<UserResponse>> RegisterParent(ParentRegisterRequest request)
        {
            var result = await _authenticationService.RegisterParentAsync(request);
            return Ok(result);
        }

        [HttpPost("RegisterParentWithInvite")]
        public async Task<IActionResult> RegisterParentWithInvite([FromBody] ParentRegisterWithInviteRequest request)
        {
            var result = await _authenticationService.RegisterParentWithInviteAsync(request);
            return Ok(result);
        }
    }
}