using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class DashboardService : IDashboardService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICourseRepository _courseRepository;
        private readonly IStudentApplicationRepository _studentApplicationRepository;
        private readonly IInstructorApplicationRepository _instructorApplicationRepository;
        private readonly IDropoutRepository _dropoutRequestRepository;

        public DashboardService(
            UserManager<ApplicationUser> userManager, 
            ICourseRepository courseRepository,
            IStudentApplicationRepository studentApplicationRepository,
            IInstructorApplicationRepository instructorApplicationRepository,
            IDropoutRepository dropoutRequestRepository)
        {
            _userManager = userManager;
            _courseRepository = courseRepository;
            _studentApplicationRepository = studentApplicationRepository;
            _instructorApplicationRepository = instructorApplicationRepository;
            _dropoutRequestRepository = dropoutRequestRepository;
        }

        public async Task<Response> GetDashboardStatsAsync()
        {
            // Count students
            var students = await _userManager.Users.OfType<Student>().ToListAsync();
            var totalStudents = students.Count;

            // Count instructors
            var instructors = await _userManager.Users.OfType<Instructor>().ToListAsync();
            var totalInstructors = instructors.Count;

            // Count active courses
            var courses = await _courseRepository.GetAllAsync();
            var activeCourses = courses.Count(c => c.Status == Status.Active);

            // Count pending student applications
            var studentApps = await _studentApplicationRepository.GetAllAsync();
            var pendingStudentApps = studentApps.Count(a => a.ApplicationStatus == ApplicationStatus.Pending);

            // Count pending instructor applications
            var instructorApps = await _instructorApplicationRepository.GetAllAsync();
            var pendingInstructorApps = instructorApps.Count(a => a.ApplicationStatus == ApplicationStatus.Pending);

            var pendingApplications = pendingStudentApps + pendingInstructorApps;

            // Count pending dropout requests
            var dropouts = await _dropoutRequestRepository.GetAllAsync();
            var pendingDropouts = dropouts.Count(d => d.ApplicationStatus == ApplicationStatus.Pending);

            var stats = new DashboardStatsResponse
            {
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                ActiveCourses = activeCourses,
                PendingApplications = pendingApplications,
                PendingDropouts = pendingDropouts
            };

            return new SuccessResponse<DashboardStatsResponse> { Data = stats };
        }
    }
}
