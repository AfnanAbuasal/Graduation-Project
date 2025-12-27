using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IChoiceService
    {
        Task<(int NumberOfEntries, Response Response)> CreateAsync(int questionId, ChoiceRequest request);
        Task<(int NumberOfEntries, Response Response)> UpdateAsync(int id, ChoiceRequest request);
        Task<(int NumberOfEntries, Response Response)> DeleteAsync(int id);
        Task<(bool Success, Response Response)> GetByIdAsync(int id);
        Task<Response> GetByQuestionIdAsync(int questionId);
    }
}