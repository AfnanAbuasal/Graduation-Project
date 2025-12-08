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

        public DashboardService(UserManager<ApplicationUser> userManager, ICourseRepository courseRepository)
        {
            _userManager = userManager;
            _courseRepository = courseRepository;
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

            var stats = new DashboardStatsResponse
            {
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                ActiveCourses = activeCourses
            };

            return new SuccessResponse<DashboardStatsResponse> { Data = stats };
        }
    }
}
