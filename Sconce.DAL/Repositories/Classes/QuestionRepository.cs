using Sconce.DAL.Data;
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
    public class QuestionRepository : GenericRepository<Question>, IQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public QuestionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Question>> GetAllByCourseIdAsync(int courseId)
        {
            return await _context.Questions
                .Where(q => q.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByCreatedByInstructorIdAsync(string instructorId)
        {
            return await _context.Questions
                .Where(q => q.CreatedByInstructorId == instructorId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Question?> GetByIdWithCourseAsync(int id)
        {
            return await _context.Questions
                .Include(q => q.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);
        }
    }
}
