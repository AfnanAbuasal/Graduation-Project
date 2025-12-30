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
    public interface ILevelService : IGenericService<LevelRequest, LevelResponse, Level>
    {
        Task<Response> GetAllByProgramAsync(int programId, bool onlyActive = false);
        Task<(int NumberOfEntries, Response Response)> IncreasePlannedCourseCountAsync(int levelId, IncreasePlannedCountRequest request);
    }
}
