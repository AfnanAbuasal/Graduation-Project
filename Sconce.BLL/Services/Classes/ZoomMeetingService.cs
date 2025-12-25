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
