using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Student
{
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Area("Student")]
    [Authorize(Roles = "Student")]
    public class ZoomMeetingsController : ControllerBase
    {
        private readonly IZoomMeetingService _zoomMeetingService;

        public ZoomMeetingsController(IZoomMeetingService zoomMeetingService)
        {
            _zoomMeetingService = zoomMeetingService;
        }

        // Shows details for a specific Zoom meeting.
        [HttpGet("{id}")]
        public async Task<ActionResult<Response>> GetById([FromRoute] int id)
        {
            var result = await _zoomMeetingService.GetByIdAsync(id);
            if (!result.Success) return BadRequest(result.Response);
            return Ok(result.Response);
        }

        // Get my Zoom meeting performance.
        [HttpGet("Performance/Section/{sectionId}")]
        public async Task<ActionResult<Response>> GetMyPerformance([FromRoute] int sectionId, [FromQuery] int? windowDays = null)
        {
            var studentId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(studentId))
                return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

            var request = new PerformanceFilterRequest
            {
                SectionId = sectionId,
                StudentId = studentId,
                WindowDays = windowDays
            };

            var result = await _zoomMeetingService.GetStudentZoomPerformanceAsync(request);
            if (result is ErrorResponse) return BadRequest(result);
            return Ok(result);
        }
    }
}
