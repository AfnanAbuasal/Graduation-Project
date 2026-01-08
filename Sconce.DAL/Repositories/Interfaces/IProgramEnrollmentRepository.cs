using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IProgramEnrollmentRepository : IGenericRepository<ProgramEnrollment>
    {
        Task<ProgramEnrollment?> GetByProgramAndStudentAsync(int programId, string studentId, bool includeProficiencyExamAttempt = false);
        Task<IEnumerable<ProgramEnrollment>> GetByProgramIdWithDetailsAsync(int programId);
        Task<(IEnumerable<ProgramEnrollment> Enrollments, int TotalCount)> GetFilteredEnrollmentsAsync(
            int programId,
            string? placementStatus = null,
            string? examStatus = null,
            int? recommendedCourseId = null,
            string sortOrder = "oldest",
            int pageNumber = 1,
            int pageSize = 10);
    }
}
