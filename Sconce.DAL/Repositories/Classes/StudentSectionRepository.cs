using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class StudentSectionRepository : IStudentSectionRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentSectionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(string studentId, int sectionId)
        {
            return await _context.StudentSections
                .AnyAsync(ss => ss.StudentId == studentId && ss.SectionId == sectionId);
        }

        public async Task<int> GetCountBySectionIdAsync(int sectionId)
        {
            return await _context.StudentSections
                .CountAsync(ss => ss.SectionId == sectionId);
        }

        public async Task<int> AddAsync(StudentSection entity)
        {
            await _context.StudentSections.AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentSection>> GetByStudentIdAsync(string studentId)
        {
            return await _context.StudentSections
                .Include(ss => ss.Section)
                    .ThenInclude(s => s.Course)
                        .ThenInclude(c => c.Level)
                            .ThenInclude(l => l.Program)
                .Where(ss => ss.StudentId == studentId)
                .ToListAsync();
        }
    }
}
