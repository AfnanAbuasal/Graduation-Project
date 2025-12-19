using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;

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
    }
}
