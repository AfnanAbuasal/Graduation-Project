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

        public override async Task<IEnumerable<Question>> GetAllAsync(bool withTracking = false)
        {
            if (withTracking)
            {
                return await _context.Questions
                    .Include(q => (q as MultipleChoiceQuestion)!.Choices)
                    .Include(q => q.Course)
                        .ThenInclude(c => c.Level)
                    .ToListAsync();
            }

            return await _context.Questions
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
                .Include(q => q.Course)
                    .ThenInclude(c => c.Level)
                .AsNoTracking()
                .ToListAsync();
        }
        public async Task<IEnumerable<Question>> GetAllByCourseIdAsync(int courseId)
        {
            return await _context.Questions
                .Where(q => q.CourseId == courseId)
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Question>> GetByCreatedByInstructorIdAsync(string instructorId)
        {
            return await _context.Questions
                .Where(q => q.CreatedByInstructorId == instructorId)
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
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
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
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

        public async Task<IEnumerable<MultipleChoiceQuestion>> GetMultipleChoiceByCourseAndSelectionModeAsync(int courseId, bool allowMultiple)
        {
            return await _context.Set<MultipleChoiceQuestion>()
                .Where(q => q.CourseId == courseId && q.AllowMultipleSelections == allowMultiple)
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

        public async Task<IEnumerable<Question>> GetAllByDifficultyAndCourseAsync(int courseId, Difficulty difficulty)
        {
            return await _context.Questions
                .Where(q => q.CourseId == courseId && q.Difficulty == difficulty)
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
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
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> CountByTypeAsync(int courseId, string type)
        {
            var questions = await _context.Questions
                .Where(q => q.CourseId == courseId)
                .Include(q => (q as MultipleChoiceQuestion)!.Choices)
                .AsNoTracking()
                .ToListAsync();

            return type switch
            {
                "mcq" => questions.OfType<MultipleChoiceQuestion>().Count(),
                "mcqonecorrect" => questions.OfType<MultipleChoiceQuestion>()
                    .Count(q => q.AllowMultipleSelections == false),
                "mcqmulticorrect" => questions.OfType<MultipleChoiceQuestion>()
                    .Count(q => q.AllowMultipleSelections == true),
                "essay" => questions.OfType<EssayQuestion>().Count(),
                _ => 0
            };
        }

        public async Task<int> CountByCourseAsync(int courseId)
        {
            return await _context.Questions
                .Where(q => q.CourseId == courseId)
                .CountAsync();
        }
    }
}
