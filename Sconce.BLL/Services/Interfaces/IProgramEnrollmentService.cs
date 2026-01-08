using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IProgramEnrollmentService
    {
        Task<(bool Success, Response Response)> EnrollStudentAsync(int programId, string studentId);
        Task<(IEnumerable<ProgramEnrollmentResponse> Enrollments, int TotalCount)> GetEnrollmentsForProgramAsync(
            int programId,
            string? placementStatus = null,
            string? examStatus = null,
            int? recommendedCourseId = null,
            string sortOrder = "oldest",
            int pageNumber = 1,
            int pageSize = 10);
        Task<(bool Success, Response Response)> SetRecommendedCourseAsync(int programId, string studentId, int recommendedCourseId);
    }
}
