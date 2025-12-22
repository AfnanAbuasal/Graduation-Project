using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class ProgramService : GenericService<ProgramRequest, ProgramResponse, Program>, IProgramService
    {
        private readonly IProgramRepository _programRepository;

        public ProgramService(IProgramRepository programRepository) : base(programRepository)
        {
            _programRepository = programRepository;
        }

        public async Task<(int NumberOfEntries, Response Response)> IncreasePlannedLevelCountAsync(int programId, IncreasePlannedCountRequest request)
        {
            // Get the program to update
            var program = await _programRepository.GetByIdAsync(programId);
            if (program == null)
                return (0, new ErrorResponse { Errors = ["Program not found."] });

            // Increase the planned level count
            program.PlannedLevelCount += request.Increment;
            program.UpdatedAt = DateTime.UtcNow;

            var rows = await _programRepository.UpdateAsync(program);

            return (rows, new SuccessResponse<string> { Data = $"Planned level count increased to {program.PlannedLevelCount}." });
        }
    }
}
