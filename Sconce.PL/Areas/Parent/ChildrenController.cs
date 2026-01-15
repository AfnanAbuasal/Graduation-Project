using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Parent
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Parent")]
    [Authorize(Roles = "Parent")]
    public class ChildrenController : ControllerBase
    {
        private readonly IParentAccessService _parentAccessService;

        public ChildrenController(IParentAccessService parentAccessService)
        {
            _parentAccessService = parentAccessService;
        }

        // Get list of confirmed children (students) for the authenticated parent.
        [HttpGet]
        public async Task<ActionResult<Response>> GetChildren()
        {
            var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var result = await _parentAccessService.GetChildrenAsync(parentId);
            if (result is ErrorResponse) 
                return BadRequest(result);
            
            return Ok(result);
        }
    }
}
