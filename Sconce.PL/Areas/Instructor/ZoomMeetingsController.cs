using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;

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

		// Lists all Zoom meetings, optionally only the active ones.
		[HttpGet]
		public async Task<ActionResult<Response>> GetAll([FromQuery] bool onlyActive = false)
		{
			var zoomMeetings = await _zoomMeetingService.GetAllAsync(onlyActive);
			return Ok(zoomMeetings);
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
	}
}
