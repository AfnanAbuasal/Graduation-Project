using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class UpcomingEventsService : IUpcomingEventsService
    {
        private readonly IAssignmentRepository _assignmentRepository;
        private readonly ISubmissionRepository _submissionRepository;
        private readonly IExamRepository _examRepository;
        private readonly IExamAttemptRepository _examAttemptRepository;
        private readonly IZoomMeetingRepository _zoomMeetingRepository;
        private readonly ISectionRepository _sectionRepository;

        public UpcomingEventsService(
            IAssignmentRepository assignmentRepository,
            ISubmissionRepository submissionRepository,
            IExamRepository examRepository,
            IExamAttemptRepository examAttemptRepository,
            IZoomMeetingRepository zoomMeetingRepository,
            ISectionRepository sectionRepository)
        {
            _assignmentRepository = assignmentRepository;
            _submissionRepository = submissionRepository;
            _examRepository = examRepository;
            _examAttemptRepository = examAttemptRepository;
            _zoomMeetingRepository = zoomMeetingRepository;
            _sectionRepository = sectionRepository;
        }

        public async Task<Response> GetStudentUpcomingEventsAsync(string studentId, int? windowDays)
        {
            // Default window is 14 days
            int window = windowDays ?? 14;
            DateTime now = DateTime.Now;
            DateTime windowEnd = now.AddDays(window);

            // Get all sections the student is enrolled in
            var studentSections = await _sectionRepository.GetStudentSectionsAsync(studentId);
            var sections = studentSections.Select(ss => ss.Section).Where(s => s != null).ToList();
            var sectionIds = sections.Select(s => s.Id).ToList();

            if (!sectionIds.Any())
            {
                return new SuccessResponse<UpcomingEventsResponse>
                {
                    Data = new UpcomingEventsResponse
                    {
                        Events = new List<UpcomingEventItemResponse>(),
                        Summary = new UpcomingEventsSummaryResponse
                        {
                            TotalEvents = 0,
                            AssignmentsCount = 0,
                            ExamsCount = 0,
                            ZoomMeetingsCount = 0
                        }
                    }
                };
            }

            var events = new List<UpcomingEventItemResponse>();

            // Fetch upcoming assignments
            var assignments = await _assignmentRepository.GetAllAsync();
            var upcomingAssignments = assignments
                .Where(a => a.SectionId.HasValue && sectionIds.Contains(a.SectionId.Value) && a.DueDate >= now && a.DueDate <= windowEnd)
                .ToList();

            foreach (var assignment in upcomingAssignments)
            {
                var submission = await _submissionRepository.GetByAssignmentAndStudentAsync(assignment.Id, studentId);
                var section = sections.FirstOrDefault(s => s.Id == assignment.SectionId);
                if (section == null) continue;

                events.Add(new UpcomingEventItemResponse
                {
                    Type = "Assignment",
                    Title = assignment.Title,
                    SectionName = section.Name,
                    EventDate = assignment.DueDate,
                    DueInDays = (int)(assignment.DueDate - now).TotalDays,
                    Submitted = submission != null,
                    Time = null
                });
            }

            // Fetch upcoming exams
            var exams = await _examRepository.GetAllAsync();
            var upcomingExams = exams
                .Where(e => e.SectionId.HasValue && sectionIds.Contains(e.SectionId.Value) && e.AvailableFrom >= now && e.AvailableFrom <= windowEnd)
                .ToList();

            // Get all attempts for the student once to avoid N+1 queries
            var allAttempts = await _examAttemptRepository.GetAllByStudentIdAsync(studentId);

            foreach (var exam in upcomingExams)
            {
                if (!exam.AvailableFrom.HasValue) continue; // Skip if AvailableFrom is null

                var hasCompletedAttempt = allAttempts.Any(a => a.ExamId == exam.Id && a.AttemptStatus != AttemptStatus.InProgress);
                var section = sections.FirstOrDefault(s => s.Id == exam.SectionId);
                if (section == null) continue;

                events.Add(new UpcomingEventItemResponse
                {
                    Type = "Exam",
                    Title = exam.Title,
                    SectionName = section.Name,
                    EventDate = exam.AvailableFrom.Value,
                    DueInDays = exam.AvailableTo.HasValue ? (int)(exam.AvailableTo.Value - now).TotalDays : -1,
                    Submitted = hasCompletedAttempt,
                    Time = null
                });
            }

            // Fetch upcoming Zoom meetings
            var zoomMeetings = await _zoomMeetingRepository.GetAllAsync();
            var upcomingZoomMeetings = zoomMeetings
                .Where(z => z.SectionId.HasValue && sectionIds.Contains(z.SectionId.Value) && z.ZoomData != null && z.ZoomData.StartTime >= now && z.ZoomData.StartTime <= windowEnd)
                .ToList();

            foreach (var zoomMeeting in upcomingZoomMeetings)
            {
                if (zoomMeeting.ZoomData == null) continue;

                var section = sections.FirstOrDefault(s => s.Id == zoomMeeting.SectionId);
                if (section == null) continue;

                events.Add(new UpcomingEventItemResponse
                {
                    Type = "ZoomMeeting",
                    Title = zoomMeeting.Title,
                    SectionName = section.Name,
                    EventDate = zoomMeeting.ZoomData.StartTime,
                    DueInDays = (int)(zoomMeeting.ZoomData.StartTime - now).TotalDays,
                    Submitted = false, // Zoom meetings don't have submissions
                    Time = zoomMeeting.ZoomData.StartTime.ToString("h:mm tt")
                });
            }

            // Sort events by event date
            var sortedEvents = events.OrderBy(e => e.EventDate).ToList();

            // Calculate summary
            var summary = new UpcomingEventsSummaryResponse
            {
                TotalEvents = sortedEvents.Count,
                AssignmentsCount = sortedEvents.Count(e => e.Type == "Assignment"),
                ExamsCount = sortedEvents.Count(e => e.Type == "Exam"),
                ZoomMeetingsCount = sortedEvents.Count(e => e.Type == "ZoomMeeting")
            };

            return new SuccessResponse<UpcomingEventsResponse>
            {
                Data = new UpcomingEventsResponse
                {
                    Events = sortedEvents,
                    Summary = summary
                }
            };
        }
    }
}
