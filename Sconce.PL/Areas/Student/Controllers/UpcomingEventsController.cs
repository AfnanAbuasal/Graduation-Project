using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System.Security.Claims;

namespace Sconce.PL.Areas.Student.Controllers
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class UpcomingEventsController : ControllerBase
    {
        private readonly IUpcomingEventsService _upcomingEventsService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UpcomingEventsController(
            IUpcomingEventsService upcomingEventsService,
            UserManager<ApplicationUser> userManager)
        {
            _upcomingEventsService = upcomingEventsService;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetUpcomingEvents([FromQuery] int? windowDays = 14)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var response = await _upcomingEventsService.GetStudentUpcomingEventsAsync(studentId, windowDays);
            return Ok(response);
        }
    }
}
