using Sconce.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IAnswerRepository : IGenericRepository<Answer>
    {
        Task<Answer?> GetByAttemptAndExamQuestionAsync(int attemptId, int examQuestionId);
        Task<IEnumerable<Answer>> GetAllByAttemptIdAsync(int attemptId);
    }
}
