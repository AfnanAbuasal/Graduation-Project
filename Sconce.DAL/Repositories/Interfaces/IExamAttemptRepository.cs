using Sconce.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IExamAttemptRepository : IGenericRepository<ExamAttempt>
    {
        Task<ExamAttempt?> GetActiveAttemptAsync(int examId, string studentId);
        Task<int> GetAttemptsCountAsync(int examId, string studentId);
        Task<ExamAttempt?> GetByIdWithExamAsync(int attemptId);
        Task<List<ExamAttempt>> GetAttemptsByExamForStudentAsync(int examId, string studentId);
    }
}
