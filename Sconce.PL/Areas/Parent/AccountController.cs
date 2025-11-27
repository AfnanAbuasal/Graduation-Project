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
        public async Task<ActionResult<Response>> RegisterParent([FromBody] ParentRegisterRequest request)
        {
            var result = await _authenticationService.RegisterParentAsync(request);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        [HttpPost("RegisterParentWithInvite")]
        public async Task<ActionResult<Response>> RegisterParentWithInvite([FromBody] ParentRegisterWithInviteRequest request)
        {
            var result = await _authenticationService.RegisterParentWithInviteAsync(request);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }
    }
}