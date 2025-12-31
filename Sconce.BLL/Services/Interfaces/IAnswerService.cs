using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IAnswerService : IFileGenericService<AnswerRequest, AnswerResponse, Answer>
    {
        Task<Response> GetMyAnswersForAttemptAsync(int attemptId);
    }
}
