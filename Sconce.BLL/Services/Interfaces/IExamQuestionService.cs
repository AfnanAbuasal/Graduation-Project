using System.Collections.Generic;
using System.Threading.Tasks;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IExamQuestionService : IGenericService<ExamQuestionRequest, ExamQuestionResponse, ExamQuestion>
    {
        Task<Response> GetAllByExamIdAsync(int examId);
        Task<(bool Success, Response Response)> ReorderAsync(int examId, List<(int ExamQuestionId, int SortOrder)> newOrder);
        Task<Response> GetAllExamQuestionDetailsForStudentAsync(int examId);
    }
}
