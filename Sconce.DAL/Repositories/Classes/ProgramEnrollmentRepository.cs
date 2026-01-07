using Sconce.DAL.Data;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class ProgramEnrollmentRepository : GenericRepository<ProgramEnrollment>, IProgramEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ProgramEnrollmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ProgramEnrollment?> GetByProgramAndStudentAsync(int programId, string studentId)
        {
            return await _context.ProgramEnrollments
                .FirstOrDefaultAsync(pe => pe.ProgramId == programId && pe.StudentId == studentId);
        }

        public override async Task<ProgramEnrollment?> GetByIdAsync(int id)
        {
            return await _context.ProgramEnrollments
                .Include(pe => pe.Program)
                .Include(pe => pe.Student)
                .Include(pe => pe.ProficiencyExamAttempt)
                .Include(pe => pe.RecommendedCourse)
                .Include(pe => pe.PlacedSection)
                .Include(pe => pe.EvaluatedByInstructor)
                .FirstOrDefaultAsync(pe => pe.Id == id);
        }

        public async Task<IEnumerable<ProgramEnrollment>> GetByProgramIdWithDetailsAsync(int programId)
        {
            return await _context.ProgramEnrollments
                .Where(pe => pe.ProgramId == programId)
                .Include(pe => pe.Program)
                .Include(pe => pe.Student)
                .Include(pe => pe.ProficiencyExamAttempt)
                .Include(pe => pe.RecommendedCourse)
                .Include(pe => pe.PlacedSection)
                .Include(pe => pe.EvaluatedByInstructor)
                .OrderByDescending(pe => pe.CreatedAt)
                .ToListAsync();
        }

        public async Task<(IEnumerable<ProgramEnrollment> Enrollments, int TotalCount)> GetFilteredEnrollmentsAsync(
            int programId,
            string? placementStatus = null,
            string? examStatus = null,
            int? recommendedCourseId = null,
            string sortOrder = "newest",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = _context.ProgramEnrollments
                .Where(pe => pe.ProgramId == programId)
                .Include(pe => pe.Program)
                .Include(pe => pe.Student)
                .Include(pe => pe.ProficiencyExamAttempt)
                .Include(pe => pe.RecommendedCourse)
                .Include(pe => pe.PlacedSection)
                .Include(pe => pe.EvaluatedByInstructor)
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(placementStatus))
            {
                if (placementStatus == "placed")
                    query = query.Where(pe => pe.PlacedSectionId.HasValue);
                else if (placementStatus == "notplaced")
                    query = query.Where(pe => !pe.PlacedSectionId.HasValue);
            }

            if (!string.IsNullOrWhiteSpace(examStatus))
            {
                switch (examStatus)
                {
                    case "inprogress":
                        query = query.Where(pe => pe.ProficiencyExamAttempt != null &&
                            pe.ProficiencyExamAttempt.AttemptStatus == AttemptStatus.InProgress);
                        break;
                    case "submitted":
                        query = query.Where(pe => pe.ProficiencyExamAttempt != null &&
                            (pe.ProficiencyExamAttempt.AttemptStatus == AttemptStatus.Submitted 
                            || pe.ProficiencyExamAttempt.AttemptStatus == AttemptStatus.Expired));
                        break;
                    case "graded":
                        query = query.Where(pe => pe.ProficiencyExamAttempt != null &&
                            pe.ProficiencyExamAttempt.AttemptStatus == AttemptStatus.Graded);
                        break;
                    case "nottaken":
                        query = query.Where(pe => pe.ProficiencyExamAttempt == null);
                        break;
                }
            }

            if (recommendedCourseId.HasValue)
            {
                query = query.Where(pe => pe.RecommendedCourseId == recommendedCourseId);
            }

            var totalCount = await query.CountAsync();

            // Apply sorting and pagination
            var enrollments = await (sortOrder == "oldest"
                ? query.OrderBy(pe => pe.CreatedAt)
                : query.OrderByDescending(pe => pe.CreatedAt))
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (enrollments, totalCount);
        }
    }
}
