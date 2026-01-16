using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System.Security.Claims;

namespace Sconce.PL.Areas.Parent.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Parent")]
    [Authorize(Roles = "Parent")]
    public class UpcomingEventsController : ControllerBase
    {
        private readonly IUpcomingEventsService _upcomingEventsService;
        private readonly IParentAccessService _parentAccessService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpcomingEventsController(
            IUpcomingEventsService upcomingEventsService,
            IParentAccessService parentAccessService,
            UserManager<ApplicationUser> userManager)
        {
            _upcomingEventsService = upcomingEventsService;
            _parentAccessService = parentAccessService;
            _userManager = userManager;
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentUpcomingEvents(string studentId, [FromQuery] int? windowDays = 14)
        {
            var parentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(parentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            // Validate parent has access to this student
            var (hasAccess, errorMessage) = await _parentAccessService.ValidateParentAccessToStudentAsync(parentId, studentId);
            if (!hasAccess)
            {
                return Forbid(errorMessage);
            }

            var response = await _upcomingEventsService.GetStudentUpcomingEventsAsync(studentId, windowDays);
            return Ok(response);
        }
    }
}
