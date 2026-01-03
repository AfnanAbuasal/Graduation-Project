using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class ExamAttemptRepository : GenericRepository<ExamAttempt>, IExamAttemptRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamAttemptRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ExamAttempt?> GetInProgressAttemptAsync(int examId, string studentId)
        {
            var attempt = await _context.Set<ExamAttempt>()
                .Include(ea => ea.Student)
                .Include(ea => ea.Answers)
                    .ThenInclude(a => a.ExamQuestion)
                        .ThenInclude(eq => eq.Question)
                .Where(ea => ea.ExamId == examId
                             && ea.StudentId == studentId
                             && ea.AttemptStatus == AttemptStatus.InProgress
                             && ea.SubmittedAt == null)
                .OrderByDescending(ea => ea.StartedAt)
                .FirstOrDefaultAsync();
            
            if (attempt?.Answers != null)
            {
                attempt.Answers = attempt.Answers
                    .OrderBy(a => a.ExamQuestion?.SortOrder ?? int.MaxValue)
                    .ToList();
            }
            
            return attempt;
        }

        public async Task<int> GetAttemptsCountAsync(int examId, string studentId)
        {
            return await _context.Set<ExamAttempt>()
                .CountAsync(ea => ea.ExamId == examId && ea.StudentId == studentId);
        }

        public async Task<ExamAttempt?> GetByIdWithExamAsync(int attemptId)
        {
            var attempt = await _context.Set<ExamAttempt>()
                .Include(ea => ea.Exam)
                .Include(ea => ea.Student)
                .Include(ea => ea.Answers)
                    .ThenInclude(a => a.ExamQuestion)
                        .ThenInclude(eq => eq.Question)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);
            
            if (attempt?.Answers != null)
            {
                attempt.Answers = attempt.Answers
                    .OrderBy(a => a.ExamQuestion?.SortOrder ?? int.MaxValue)
                    .ToList();
            }
            
            return attempt;
        }

        public async Task<List<ExamAttempt>> GetAttemptsByExamForStudentAsync(int examId, string studentId)
        {
            var attempts = await _context.Set<ExamAttempt>()
                .Include(ea => ea.Exam)
                .Include(ea => ea.Student)
                .Include(ea => ea.Answers)
                    .ThenInclude(a => a.ExamQuestion)
                        .ThenInclude(eq => eq.Question)
                .Where(ea => ea.ExamId == examId && ea.StudentId == studentId)
                .OrderBy(ea => ea.AttemptNumber)
                .ToListAsync();
            
            // Sort answers within each attempt by question order
            foreach (var attempt in attempts)
            {
                if (attempt.Answers != null)
                {
                    attempt.Answers = attempt.Answers
                        .OrderBy(a => a.ExamQuestion?.SortOrder ?? int.MaxValue)
                        .ToList();
                }
            }
            
            return attempts;
        }

        public async Task<IEnumerable<ExamAttempt>> GetAllByExamIdAsync(int examId)
        {
            var attempts = await _context.Set<ExamAttempt>()
                .Include(ea => ea.Student)
                .Include(ea => ea.Answers)
                    .ThenInclude(a => a.ExamQuestion)
                        .ThenInclude(eq => eq.Question)
                .Where(ea => ea.ExamId == examId)
                .OrderByDescending(ea => ea.SubmittedAt)
                .ThenByDescending(ea => ea.StartedAt)
                .ToListAsync();
            
            // Sort answers within each attempt by question order
            foreach (var attempt in attempts)
            {
                if (attempt.Answers != null)
                {
                    attempt.Answers = attempt.Answers
                        .OrderBy(a => a.ExamQuestion?.SortOrder ?? int.MaxValue)
                        .ToList();
                }
            }
            
            return attempts;
        }
    }
}
