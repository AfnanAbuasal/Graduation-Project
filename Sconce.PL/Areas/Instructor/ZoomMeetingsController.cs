using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Security.Claims;

namespace Sconce.PL.Areas.Instructor
{
	[Route("api/[area]/[controller]")]
	[ApiController]
	[Area("Instructor")]
	[Authorize(Roles = "Instructor")]
	public class ZoomMeetingsController : ControllerBase
	{
		private readonly IZoomMeetingService _zoomMeetingService;

		public ZoomMeetingsController(IZoomMeetingService zoomMeetingService)
		{
			_zoomMeetingService = zoomMeetingService;
		}

		// Gets all Zoom meetings for a section (scoped to instructor's section).
		[HttpGet("Section/{sectionId}")]
		public async Task<ActionResult<Response>> GetBySection([FromRoute] int sectionId)
		{
			var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(instructorId))
				return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

			var result = await _zoomMeetingService.GetAllBySectionAsync(sectionId, instructorId);
			if (result is ErrorResponse) return BadRequest(result);
			return Ok(result);
		}

		// Shows details for a specific Zoom meeting.
		[HttpGet("{id}")]
		public async Task<ActionResult<Response>> GetById([FromRoute] int id)
		{
			var result = await _zoomMeetingService.GetByIdAsync(id);
			if (!result.Success) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Creates a new Zoom meeting.
		[HttpPost]
		public async Task<ActionResult<Response>> Create([FromBody] ZoomMeetingRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _zoomMeetingService.CreateAsync(request);
			if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Updates an existing Zoom meeting.
		[HttpPut("{id}")]
		public async Task<ActionResult<Response>> Update([FromRoute] int id, [FromBody] ZoomMeetingRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _zoomMeetingService.UpdateAsync(id, request);
			if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Deletes a Zoom meeting.
		[HttpDelete("{id}")]
		public async Task<ActionResult<Response>> Delete([FromRoute] int id)
		{
			var result = await _zoomMeetingService.DeleteAsync(id);
			if (result.NumberOfEntries <= 0) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Enables or disables a Zoom meeting.
		[HttpPatch("{id}/ToggleStatus")]
		public async Task<ActionResult<Response>> ToggleStatus([FromRoute] int id)
		{
			var result = await _zoomMeetingService.ToggleStatusAsync(id);
			if (!result.Success) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Mark student attendance for a Zoom meeting.
		[HttpPost("MarkAttendance")]
		public async Task<ActionResult<Response>> MarkAttendance([FromBody] MarkZoomAttendanceRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(instructorId))
				return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

			var result = await _zoomMeetingService.MarkAttendanceAsync(request, instructorId);
			if (!result.Success) return BadRequest(result.Response);
			return Ok(result.Response);
		}

		// Get student's Zoom meeting performance.
		[HttpPost("Performance")]
		public async Task<ActionResult<Response>> GetStudentPerformance([FromBody] PerformanceFilterRequest request)
		{
			if (!ModelState.IsValid)
				return BadRequest(ModelState);

			var result = await _zoomMeetingService.GetStudentZoomPerformanceAsync(request);
			if (result is ErrorResponse) return BadRequest(result);
			return Ok(result);
		}

		// Get attendance list for a Zoom meeting (all students with their attendance status).
		[HttpGet("{zoomMeetingId}/Attendance")]
		public async Task<ActionResult<Response>> GetAttendanceList([FromRoute] int zoomMeetingId)
		{
			var instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			if (string.IsNullOrEmpty(instructorId))
				return Unauthorized(new ErrorResponse { Errors = ["User not authenticated."] });

			var result = await _zoomMeetingService.GetAttendanceListByZoomMeetingAsync(zoomMeetingId, instructorId);
			if (result is ErrorResponse) return BadRequest(result);
			return Ok(result);
		}
	}
}
