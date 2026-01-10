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

        public async Task<ExamQuestion?> GetExamQuestionWithMcqChoicesAsync(int examQuestionId)
        {
            var examQuestion = await _context.Set<ExamQuestion>()
                .Include(eq => eq.Question)
                .FirstOrDefaultAsync(eq => eq.Id == examQuestionId);

            if (examQuestion?.Question is MultipleChoiceQuestion mcQuestion)
            {
                await _context.Entry(mcQuestion)
                    .Collection(q => q.Choices)
                    .LoadAsync();
            }

            return examQuestion;
        }

        public async Task<Answer?> GetByIdWithAttemptAndQuestionAsync(int answerId)
        {
            return await _context.Set<Answer>()
                .Include(a => a.ExamAttempt)
                    .ThenInclude(ea => ea.Exam)
                .Include(a => a.ExamQuestion)
                    .ThenInclude(eq => eq.Question)
                .FirstOrDefaultAsync(a => a.Id == answerId);
        }
    }
}
