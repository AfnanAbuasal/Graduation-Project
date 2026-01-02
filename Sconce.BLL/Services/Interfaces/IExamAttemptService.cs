using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IExamAttemptService
    {
        Task<(bool Success, Response Response)> StartAttemptAsync(int examId);
        Task<Response> GetMyAttemptsAsync(int examId);
        Task<(bool Success, Response Response)> SubmitAttemptAsync(int attemptId);
        Task<Response> GetAttemptsByExamIdAsync(int examId);
        Task<(bool Success, Response Response)> GetAttemptDetailsAsync(int attemptId);
    }
}
