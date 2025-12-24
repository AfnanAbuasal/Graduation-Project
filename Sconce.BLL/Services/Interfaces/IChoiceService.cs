using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IChoiceService
    {
        Task<(int NumberOfEntries, Response Response)> CreateAsync(ChoiceRequest request);
        Task<(int NumberOfEntries, Response Response)> UpdateAsync(int questionId, string text, ChoiceRequest request);
        Task<(int NumberOfEntries, Response Response)> DeleteAsync(int questionId, string text);
        Task<(bool Success, Response Response)> GetByIdAsync(int questionId, string text);
        Task<Response> GetByQuestionIdAsync(int questionId);
    }
}