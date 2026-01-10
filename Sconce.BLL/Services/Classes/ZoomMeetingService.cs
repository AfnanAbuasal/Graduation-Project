using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes;

public class ZoomMeetingService : GenericService<ZoomMeetingRequest, ZoomMeetingResponse, ZoomMeeting>, IZoomMeetingService
{
	private readonly IZoomMeetingRepository _zoomMeetingRepository;
	private readonly ISectionRepository _sectionRepository;

	public ZoomMeetingService(
		IZoomMeetingRepository zoomMeetingRepository,
		ISectionRepository sectionRepository)
		: base(zoomMeetingRepository)
	{
		_zoomMeetingRepository = zoomMeetingRepository;
		_sectionRepository = sectionRepository;
	}

	public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(ZoomMeetingRequest request)
	{
		var result = await base.CreateAsync(request);
		
		if (result.NumberOfEntries > 0)
		{
			await UpdateSectionTimestampAsync(request.SectionId);
		}
		
		return result;
	}

	public override async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ZoomMeetingRequest request)
	{
		var zoomMeeting = await _zoomMeetingRepository.GetByIdAsync(id);
		if (zoomMeeting == null)
			return (0, new ErrorResponse { Errors = ["Not Found."] });

		var sectionId = zoomMeeting.SectionId;
		var result = await base.UpdateAsync(id, request);
		
		if (result.NumberOfEntries > 0 && sectionId.HasValue)
		{
			await UpdateSectionTimestampAsync(sectionId.Value);
		}
		
		return result;
	}

	public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id)
	{
		var zoomMeeting = await _zoomMeetingRepository.GetByIdAsync(id);
		if (zoomMeeting == null)
			return (0, new ErrorResponse { Errors = ["Not Found."] });

		var sectionId = zoomMeeting.SectionId;
		var result = await base.DeleteAsync(id);
		
		if (result.NumberOfEntries > 0 && sectionId.HasValue)
		{
			await UpdateSectionTimestampAsync(sectionId.Value);
		}
		
		return result;
	}

	private async Task UpdateSectionTimestampAsync(int sectionId)
	{
		var section = await _sectionRepository.GetByIdAsync(sectionId);
		if (section != null)
		{
			section.UpdatedAt = DateTime.UtcNow;
			await _sectionRepository.UpdateAsync(section);
		}
	}

	public async Task<Response> GetAllBySectionAsync(int sectionId, string instructorId)
	{
		// Verify section exists
		var section = await _sectionRepository.GetByIdAsync(sectionId);
		if (section == null)
			return new ErrorResponse { Errors = ["Section not found."] };

		// Verify section is assigned to the instructor
		if (section.InstructorId != instructorId)
			return new ErrorResponse { Errors = ["Unauthorized access to this section."] };

		// Get all zoom meetings for this section
		var zoomMeetings = await _zoomMeetingRepository.GetAllBySectionIdAsync(sectionId, withTracking: false);

		return new SuccessResponse<IEnumerable<ZoomMeetingResponse>> { Data = zoomMeetings.Adapt<IEnumerable<ZoomMeetingResponse>>() };
	}
}
