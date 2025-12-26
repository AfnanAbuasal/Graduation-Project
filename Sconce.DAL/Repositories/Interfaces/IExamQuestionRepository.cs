using System.Collections.Generic;
using System.Threading.Tasks;
using Sconce.DAL.Models;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IExamQuestionRepository : IGenericRepository<ExamQuestion>
    {
        Task<IEnumerable<ExamQuestion>> GetAllByExamIdAsync(int examId);
        Task<ExamQuestion?> GetByExamAndQuestionAsync(int examId, int questionId);
        Task<bool> ExistsQuestionInExamAsync(int examId, int questionId, int? excludeId = null);
        Task<bool> ExistsSortOrderInExamAsync(int examId, int sortOrder, int? excludeId = null);
        Task<IEnumerable<ExamQuestion>> GetAllByExamIdWithQuestionAsync(int examId); // Include Question
        Task<IEnumerable<ExamQuestion>> GetAllDetailsByExamIdAsync(int examId);
    }
}
