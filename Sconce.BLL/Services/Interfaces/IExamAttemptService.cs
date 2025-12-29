using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IExamAttemptService
    {
        Task<(bool Success, Response Response)> StartAttemptAsync(StartExamAttemptRequest request);
        Task<Response> GetMyAttemptsAsync(int examId);
        Task<(bool Success, Response Response)> SubmitAttemptAsync(int attemptId);
    }
}
