using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IProgramService : IGenericService<ProgramRequest, ProgramResponse, Program>
    {
        Task<(int NumberOfEntries, Response Response)> IncreasePlannedLevelCountAsync(int programId, IncreasePlannedCountRequest request);
        Task<(bool Success, Response Response)> AssignExamWriterInstructorAsync(int programId, string instructorId);
        Task<(bool Success, Response Response)> AssignEvaluatorInstructorAsync(int programId, string instructorId);
        Task<Response> GetProgramsForExamWriterAsync();
        Task<Response> GetProgramsForEvaluatorAsync();
        Task<(bool Success, Response Response)> AssignProficiencyExamAsync(int programId, int examId);
        Task<Response> GetProgramsForStudentAsync();
        Task<Response> GetProficiencyExamForProgramAsync(int programId);
    }
}
