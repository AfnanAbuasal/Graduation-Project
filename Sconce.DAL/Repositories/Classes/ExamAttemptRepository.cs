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
            return await _context.Set<ExamAttempt>()
                .Include(ea => ea.Answers)
                .Where(ea => ea.ExamId == examId
                             && ea.StudentId == studentId
                             && ea.AttemptStatus == AttemptStatus.InProgress
                             && ea.SubmittedAt == null)
                .OrderByDescending(ea => ea.StartedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<int> GetAttemptsCountAsync(int examId, string studentId)
        {
            return await _context.Set<ExamAttempt>()
                .CountAsync(ea => ea.ExamId == examId && ea.StudentId == studentId);
        }

        public async Task<ExamAttempt?> GetByIdWithExamAsync(int attemptId)
        {
            return await _context.Set<ExamAttempt>()
                .Include(ea => ea.Exam)
                .Include(ea => ea.Answers)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);
        }

        public async Task<List<ExamAttempt>> GetAttemptsByExamForStudentAsync(int examId, string studentId)
        {
            return await _context.Set<ExamAttempt>()
                .Include(ea => ea.Exam)
                .Where(ea => ea.ExamId == examId && ea.StudentId == studentId)
                .OrderBy(ea => ea.AttemptNumber)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamAttempt>> GetAllByExamIdAsync(int examId)
        {
            return await _context.Set<ExamAttempt>()
                .Include(ea => ea.Student)
                .Where(ea => ea.ExamId == examId)
                .OrderByDescending(ea => ea.SubmittedAt)
                .ThenByDescending(ea => ea.StartedAt)
                .ToListAsync();
        }

        public async Task<ExamAttempt?> GetByIdWithDetailsAsync(int attemptId)
        {
            var attempt = await _context.Set<ExamAttempt>()
                .AsNoTracking()
                .Include(ea => ea.Exam)
                    .ThenInclude(e => e.ExamQuestions.OrderBy(eq => eq.SortOrder))
                        .ThenInclude(eq => eq.Question)
                .Include(ea => ea.Student)
                .Include(ea => ea.Answers)
                    .ThenInclude(a => a.ExamQuestion)
                .FirstOrDefaultAsync(ea => ea.Id == attemptId);

            if (attempt != null)
            {
                // Load choices for MCQ questions
                foreach (var examQuestion in attempt.Exam.ExamQuestions)
                {
                    if (examQuestion.Question is MultipleChoiceQuestion mcq)
                    {
                        await _context.Entry(mcq)
                            .Collection(q => q.Choices)
                            .LoadAsync();
                    }
                }
            }

            return attempt;
        }
    }
}
