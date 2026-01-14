using Mapster;
using Microsoft.AspNetCore.Identity;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
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
	private readonly IStudentSectionRepository _studentSectionRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public ZoomMeetingService(
		IZoomMeetingRepository zoomMeetingRepository,
		ISectionRepository sectionRepository,
		IZoomAttendanceRepository attendanceRepository,
		IStudentSectionRepository studentSectionRepository,
		UserManager<ApplicationUser> userManager)
		: base(zoomMeetingRepository)
	{
		_zoomMeetingRepository = zoomMeetingRepository;
		_sectionRepository = sectionRepository;
		_attendanceRepository = attendanceRepository;
		_studentSectionRepository = studentSectionRepository;
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

	public async Task<Response> GetStudentZoomPerformanceAsync(PerformanceFilterRequest request)
	{
		// Verify section exists
		var section = await _sectionRepository.GetByIdAsync(request.SectionId);
		if (section == null)
			return new ErrorResponse { Errors = ["Section not found."] };

		// Verify student exists and is enrolled in the section
		var studentSections = await _sectionRepository.GetStudentSectionsAsync(request.StudentId);
		var studentSection = studentSections.FirstOrDefault(ss => ss.SectionId == request.SectionId);
		if (studentSection == null)
			return new ErrorResponse { Errors = ["Student is not enrolled in this section."] };

		// Calculate time window
		DateTime windowStart;
		if (request.WindowDays.HasValue)
		{
			windowStart = DateTime.UtcNow.AddDays(-request.WindowDays.Value);
		}
		else
		{
			// Use section enrollment date as start
			windowStart = studentSection.AddedAt; // this or section.Course.StartDate?
		}

		// Get all zoom meetings in the section within the time window (past meetings only)
		var allZoomMeetings = await _zoomMeetingRepository.GetAllBySectionIdAsync(request.SectionId, withTracking: false);
		var pastZoomMeetings = allZoomMeetings
			.Where(zm => zm.ZoomData != null && 
			             zm.ZoomData.StartTime >= windowStart && 
			             zm.ZoomData.StartTime <= DateTime.UtcNow)
			.OrderBy(zm => zm.ZoomData.StartTime)
			.ToList();

		// Get all attendance records for this student in this section
		var attendanceRecords = await _attendanceRepository.GetByStudentIdAsync(request.StudentId, withTracking: false);
		var attendanceDict = attendanceRecords.ToDictionary(a => a.ZoomMeetingId, a => a);

		// Build performance items
		var performanceItems = pastZoomMeetings.Select(zm =>
		{
			var hasAttendance = attendanceDict.TryGetValue(zm.Id, out var attendance);
			return new ZoomMeetingPerformanceItemResponse
			{
				ZoomMeetingId = zm.Id,
				Title = zm.Title,
				ScheduledTime = zm.ZoomData.StartTime,
				Attended = hasAttendance ? attendance.Attended : null,
				RecordedAt = hasAttendance ? attendance.RecordedAt : null
			};
		}).ToList();

		// Calculate summary statistics
		var totalMeetings = performanceItems.Count;
		var attendedCount = performanceItems.Count(p => p.Attended == AttendanceStatus.Attended);
		var missedCount = performanceItems.Count(p => p.Attended == AttendanceStatus.Absent);
		var excusedCount = performanceItems.Count(p => p.Attended == AttendanceStatus.Excused);
		var attendanceNotMarkedCount = performanceItems.Count(p => p.Attended == null);

		// Attendance rate: attended / (attended + missed) - only counting marked meetings
		var markedMeetings = attendedCount + missedCount + excusedCount;
		var attendanceRate = markedMeetings > 0 ? (decimal)attendedCount / markedMeetings * 100 : 0;

		var summary = new ZoomMeetingPerformanceSummaryResponse
		{
			TotalMeetings = totalMeetings,
			AttendedCount = attendedCount,
			MissedCount = missedCount,
			AttendanceNotMarkedCount = attendanceNotMarkedCount,
			ExcusedCount = excusedCount,
			AttendanceRate = Math.Round(attendanceRate, 2)
		};

		var performanceResponse = new ZoomMeetingPerformanceResponse
		{
			ZoomMeetings = performanceItems,
			Summary = summary
		};

		return new SuccessResponse<ZoomMeetingPerformanceResponse> { Data = performanceResponse };
	}

	public async Task<Response> GetAttendanceListByZoomMeetingAsync(int zoomMeetingId, string instructorId)
	{
		// Get zoom meeting with section info
		var zoomMeeting = await _zoomMeetingRepository.GetByIdAsync(zoomMeetingId);
		if (zoomMeeting == null)
			return new ErrorResponse { Errors = ["Zoom meeting not found."] };

		if (!zoomMeeting.SectionId.HasValue)
			return new ErrorResponse { Errors = ["Zoom meeting is not associated with a section."] };

		// Verify section exists and instructor teaches it
		var section = await _sectionRepository.GetByIdAsync(zoomMeeting.SectionId.Value);
		if (section == null)
			return new ErrorResponse { Errors = ["Section not found."] };

		if (section.InstructorId != instructorId)
			return new ErrorResponse { Errors = ["Unauthorized. You do not teach this section."] };

		// Get all students enrolled in the section
		var students = await _studentSectionRepository.GetStudentsBySectionIdAsync(zoomMeeting.SectionId.Value);

		// Get all attendance records for this zoom meeting
		var attendanceRecords = await _attendanceRepository.GetByZoomMeetingIdAsync(zoomMeetingId, withTracking: false);
		var attendanceDict = attendanceRecords.ToDictionary(a => a.StudentId, a => a);

		// Build response list with all students and their attendance status
		var responseList = students
			.Select(student =>
			{
				var hasAttendance = attendanceDict.TryGetValue(student.Id, out var attendance);
				return new StudentZoomAttendanceResponse
				{
					StudentId = student.Id,
					StudentName = student.FullName,
					AttendanceStatus = hasAttendance ? attendance.Attended : null,
					RecordedAt = hasAttendance ? attendance.RecordedAt : null
				};
			})
			.OrderBy(s => s.StudentName) // Sort alphabetically by name
			.ToList();

		return new SuccessResponse<IEnumerable<StudentZoomAttendanceResponse>> { Data = responseList };
	}
}
