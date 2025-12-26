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
    public class ChoiceRepository : IChoiceRepository
    {
        private readonly ApplicationDbContext _context;

        public ChoiceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(Choice entity)
        {
            await _context.Set<Choice>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(Choice entity)
        {
            _context.Set<Choice>().Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> DeleteAsync(Choice entity)
        {
            _context.Set<Choice>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<Choice?> GetByIdAsync(int questionId, string text)
        {
            // Composite key lookup (QuestionId, Text)
            return await _context.Set<Choice>().FindAsync(questionId, text);
        }

        public async Task<IEnumerable<Choice>> GetByQuestionIdAsync(int questionId)
        {
            return await _context.Set<Choice>()
                .Where(c => c.QuestionId == questionId)
                .OrderBy(c => c.Text)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Choice>> GetByQuestionIdsAsync(IEnumerable<int> questionIds)
        {
            return await _context.Set<Choice>()
                .Where(c => questionIds.Contains(c.QuestionId))
                .OrderBy(c => c.QuestionId)
                .ThenBy(c => c.Text)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(int questionId, string text)
        {
            return await _context.Set<Choice>()
                .AnyAsync(c => c.QuestionId == questionId && c.Text == text);
        }

        public async Task<int> CountByQuestionAsync(int questionId)
        {
            return await _context.Set<Choice>()
                .CountAsync(c => c.QuestionId == questionId);
        }
    }
}