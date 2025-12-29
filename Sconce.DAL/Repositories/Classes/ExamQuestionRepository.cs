using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class ExamQuestionRepository : GenericRepository<ExamQuestion>, IExamQuestionRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamQuestionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ExamQuestion>> GetAllByExamIdAsync(int examId)
        {
            return await _context.ExamQuestions
                .Where(eq => eq.ExamId == examId)
                .OrderBy(eq => eq.SortOrder)
                .ToListAsync();
        }

        public async Task<ExamQuestion?> GetByExamAndQuestionAsync(int examId, int questionId)
        {
            return await _context.ExamQuestions
                .FirstOrDefaultAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId);
        }

        public async Task<bool> ExistsQuestionInExamAsync(int examId, int questionId, int? excludeId = null)
        {
            return await _context.ExamQuestions
                .AnyAsync(eq => eq.ExamId == examId && eq.QuestionId == questionId && eq.Id != excludeId);
        }

        public async Task<bool> ExistsSortOrderInExamAsync(int examId, int sortOrder, int? excludeId = null)
        {
            return await _context.ExamQuestions
                .AnyAsync(eq => eq.ExamId == examId && eq.SortOrder == sortOrder && eq.Id != excludeId);
        }

        public async Task<bool> ExistsQuestionInAnyExamAsync(int questionId)
        {
            return await _context.ExamQuestions
                .AnyAsync(eq => eq.QuestionId == questionId);
        }

        public async Task<IEnumerable<ExamQuestion>> GetAllByExamIdWithQuestionAsync(int examId)
        {
            return await _context.ExamQuestions
                .Include(eq => eq.Question)
                .Where(eq => eq.ExamId == examId)
                .OrderBy(eq => eq.SortOrder)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamQuestion>> GetAllDetailsByExamIdAsync(int examId)
        {
            return await _context.ExamQuestions
                .Include(eq => eq.Question)
                .Where(eq => eq.ExamId == examId)
                .OrderBy(eq => eq.SortOrder)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
