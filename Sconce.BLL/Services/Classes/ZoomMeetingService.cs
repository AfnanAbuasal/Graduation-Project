using Mapster;
using Microsoft.AspNetCore.Identity;
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
	private readonly IZoomAttendanceRepository _attendanceRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ZoomMeetingService(
		IZoomMeetingRepository zoomMeetingRepository,
		ISectionRepository sectionRepository,
		IZoomAttendanceRepository attendanceRepository,
		UserManager<ApplicationUser> userManager)
		: base(zoomMeetingRepository)
	{
		_zoomMeetingRepository = zoomMeetingRepository;
		_sectionRepository = sectionRepository;
		_attendanceRepository = attendanceRepository;
        _userManager = userManager;
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

	public async Task<(bool Success, Response Response)> MarkAttendanceAsync(MarkZoomAttendanceRequest request, string instructorId)
	{
		// Get zoom meeting
		var zoomMeeting = await _zoomMeetingRepository.GetByIdAsync(request.ZoomMeetingId);
		if (zoomMeeting == null)
			return (false, new ErrorResponse { Errors = ["Zoom meeting not found."] });

		// Verify section exists and instructor teaches it
		var section = await _sectionRepository.GetByIdAsync(zoomMeeting.SectionId!.Value);
		if (section == null)
			return (false, new ErrorResponse { Errors = ["Section not found."] });

		if (section.InstructorId != instructorId)
			return (false, new ErrorResponse { Errors = ["Unauthorized. You do not teach this section."] });

		// Verify student exists and is enrolled in the section
		var student = await _userManager.FindByIdAsync(request.StudentId);
		if (student == null)
			return (false, new ErrorResponse { Errors = ["Student not found."] });

		// Check if student is enrolled in the section
		var studentSections = await _sectionRepository.GetStudentSectionsAsync(request.StudentId);
		if (!studentSections.Any(ss => ss.SectionId == section.Id))
			return (false, new ErrorResponse { Errors = ["Student is not enrolled in this section."] });

		// Get or create attendance record
		var existingAttendance = await _attendanceRepository.GetByZoomMeetingAndStudentAsync(request.ZoomMeetingId, request.StudentId, withTracking: true);

		if (existingAttendance != null)
		{
			// Update existing record
			existingAttendance.Attended = request.Attended;
			existingAttendance.RecordedAt = DateTime.UtcNow;
			await _attendanceRepository.UpdateAsync(existingAttendance);
		}
		else
		{
			// Create new record
			var attendance = new ZoomAttendance
			{
				ZoomMeetingId = request.ZoomMeetingId,
				StudentId = request.StudentId,
				Attended = request.Attended,
				RecordedAt = DateTime.UtcNow,
			};
			await _attendanceRepository.AddAsync(attendance);
		}

		var response = new ZoomAttendanceResponse
		{
			ZoomMeetingId = request.ZoomMeetingId,
			StudentId = request.StudentId,
			StudentName = student.FullName,
			Attended = request.Attended,
			RecordedAt = DateTime.UtcNow
		};

		return (true, new SuccessResponse<ZoomAttendanceResponse> { Data = response });
	}
}
