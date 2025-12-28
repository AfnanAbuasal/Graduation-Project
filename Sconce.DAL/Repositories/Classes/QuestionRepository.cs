using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
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
                .Include(q => (q as MultipleChoiceQuestion).Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByCreatedByInstructorIdAsync(string instructorId)
        {
            return await _context.Questions
                .Where(q => q.CreatedByInstructorId == instructorId)
                .Include(q => (q as MultipleChoiceQuestion).Choices)
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

        public async Task<IEnumerable<Question>> GetByIdsAsync(IEnumerable<int> ids)
        {
            var idList = ids?.ToList() ?? new List<int>();
            if (!idList.Any())
                return Enumerable.Empty<Question>();

            return await _context.Questions
                .Where(q => idList.Contains(q.Id))
                .Include(q => (q as MultipleChoiceQuestion).Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<MultipleChoiceQuestion>> GetAllMultipleChoiceByCourseIdAsync(int courseId)
        {
            return await _context.Set<MultipleChoiceQuestion>()
                .Where(q => q.CourseId == courseId)
                .Include(q => q.Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<EssayQuestion>> GetAllEssayByCourseIdAsync(int courseId)
        {
            return await _context.Set<EssayQuestion>()
                .Where(q => q.CourseId == courseId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<MultipleChoiceQuestion?> GetMultipleChoiceByIdAsync(int id)
        {
            return await _context.Set<MultipleChoiceQuestion>()
                .Include(q => q.Choices)
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<EssayQuestion?> GetEssayByIdAsync(int id)
        {
            return await _context.Set<EssayQuestion>()
                .AsNoTracking()
                .FirstOrDefaultAsync(q => q.Id == id);
        }

        public async Task<IEnumerable<Question>> GetAllByTypeAsync<TQuestion>() where TQuestion : Question
        {
            var query = _context.Set<TQuestion>().AsNoTracking();

            if (typeof(TQuestion) == typeof(MultipleChoiceQuestion))
            {
                // Include choices when fetching MCQs
                return await _context.Set<MultipleChoiceQuestion>()
                    .Include(q => q.Choices)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetAllByDifficultyAsync(Difficulty difficulty)
        {
            return await _context.Questions
                .Where(q => q.Difficulty == difficulty)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> SearchByPromptAsync(int courseId, string term)
        {
            term = term?.Trim() ?? string.Empty;
            if (string.IsNullOrEmpty(term))
            {
                return Enumerable.Empty<Question>();
            }

            return await _context.Questions
                .Where(q => q.CourseId == courseId && q.Prompt.Contains(term))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountByCourseAsync(int courseId)
        {
            return await _context.Questions
                .CountAsync(q => q.CourseId == courseId);
        }

        public async Task<int> CountByTypeAsync<TQuestion>(int courseId) where TQuestion : Question
        {
            return await _context.Set<TQuestion>()
                .CountAsync(q => q.CourseId == courseId);
        }
    }
}
