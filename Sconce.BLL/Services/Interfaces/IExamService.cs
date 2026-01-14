using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IExamService : IGenericService<ExamRequest, ExamResponse, Exam>
    {
        Task<Response> GetAllBySectionAsync(int sectionId, string instructorId, bool onlyActive = false);
        Task<Response> GetAllByProgramAsync(int programId, string instructorId, bool onlyActive = false);
        Task<(bool Success, Response Response)> ChangeExamStatusAsync(int id, ExamStatus newStatus);
        Task<(bool Success, Response Response)> GetExamStatusAsync(int id);
        Task<(bool Success, Response Response)> ReopenProficiencyExamAsync(int id);
        Task<Response> GetPublishedBySectionForStudentAsync(int sectionId, string studentId);
    }
}
