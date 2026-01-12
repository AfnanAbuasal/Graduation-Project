using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.Repositories.Classes
{
    public class SectionRepository : GenericRepository<Section>, ISectionRepository
    {
        private readonly ApplicationDbContext _context;

        public SectionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Section?> GetByIdWithInstructorAsync(int id)
        {
            return await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Section?> GetByIdWithCourseAsync(int id)
        {
            return await _context.Sections
                .Include(s => s.Course)
                    .ThenInclude(c => c.Level)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Section>> GetAllWithInstructorAsync()
        {
            return await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Section>> GetByInstructorIdWithInstructorAsync(string instructorId)
        {
            return await _context.Sections
                .Include(s => s.Course)
                .Include(s => s.Instructor)
                .Where(s => s.InstructorId == instructorId)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId, bool onlyActive = false)
        {
            var query = _context.Sections
                .Where(s => s.CourseId == courseId)
                .Include(s => s.Course)
                .Include(s => s.Instructor);

            if (onlyActive)
            {
                query = (Microsoft.EntityFrameworkCore.Query.IIncludableQueryable<Section, Instructor?>)query.Where(s => s.Status == Status.Active);
            }

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentSection>> GetStudentSectionsAsync(string studentId)
        {
            return await _context.StudentSections
                .Where(ss => ss.StudentId == studentId)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
