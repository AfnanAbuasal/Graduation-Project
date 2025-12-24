using Sconce.DAL.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IChoiceRepository
    {
        Task<int> AddAsync(Choice entity);
        Task<int> UpdateAsync(Choice entity);
        Task<int> DeleteAsync(Choice entity);

        Task<Choice?> GetByIdAsync(int questionId, string text);
        Task<IEnumerable<Choice>> GetByQuestionIdAsync(int questionId);
        Task<bool> ExistsAsync(int questionId, string text);
        Task<int> CountByQuestionAsync(int questionId);
    }
}