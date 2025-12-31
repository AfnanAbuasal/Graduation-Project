using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class AnswerRepository : GenericRepository<Answer>, IAnswerRepository
    {
        private readonly ApplicationDbContext _context;

        public AnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Answer?> GetByAttemptAndExamQuestionAsync(int attemptId, int examQuestionId)
        {
            return await _context.Set<Answer>()
                .FirstOrDefaultAsync(a => a.ExamAttemptId == attemptId 
                                       && a.ExamQuestionId == examQuestionId);
        }

        public async Task<IEnumerable<Answer>> GetAllByAttemptIdAsync(int attemptId)
        {
            return await _context.Set<Answer>()
                .Include(a => a.ExamQuestion)
                .ThenInclude(eq => eq.Question)
                .Where(a => a.ExamAttemptId == attemptId)
                .OrderBy(a => a.ExamQuestion.SortOrder)
                .ToListAsync();
        }
    }
}
