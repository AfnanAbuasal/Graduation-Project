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
    }
}
