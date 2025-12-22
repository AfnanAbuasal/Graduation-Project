using Mapster;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class LevelService : GenericService<LevelRequest, LevelResponse, Level>, ILevelService
    {
        private readonly ILevelRepository _levelRepository;
        private readonly IProgramRepository _programRepository;

        public LevelService(ILevelRepository levelRepository, IProgramRepository programRepository) : base(levelRepository)
        {
            _levelRepository = levelRepository;
            _programRepository = programRepository;
        }

        public override async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var list = await _levelRepository.GetAllWithProgramAsync();

            if (onlyActive)
                list = list.Where(x => x.Status == Status.Active);

            var responseList = new List<LevelResponse>();
            foreach (var entity in list)
            {
                var dto = entity.Adapt<LevelResponse>();
                dto.ProgramName = entity.Program?.Name;
                responseList.Add(dto);
            }

            return new SuccessResponse<IEnumerable<LevelResponse>> { Data = responseList };
        }

        public override async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _levelRepository.GetByIdWithProgramAsync(Id);
            if (entity == null)
                return (false, new ErrorResponse { Errors = ["Not Found."] });

            var dto = entity.Adapt<LevelResponse>();
            dto.ProgramName = entity.Program?.Name;
            return (true, new SuccessResponse<LevelResponse> { Data = dto });
        }

        public override async Task<(int NumberOfEntries, Response Response)> CreateAsync(LevelRequest request)
        {
            // Ensure Program exists before creating level
            var program = await _programRepository.GetByIdAsync(request.ProgramId);
            if (program == null)
                return (0, new ErrorResponse { Errors = [$"Program with Id: {request.ProgramId} not found."] });

            // Validate that program has not reached its planned level count
            if (program.ActualLevelCount >= program.PlannedLevelCount)
                return (0, new ErrorResponse { Errors = [
                    $"Cannot create level. Program has reached its planned level count ({program.PlannedLevelCount})."
                ] });

            var level = request.Adapt<Level>();
            var rows = await _levelRepository.AddAsync(level);

            // Increment ActualLevelCount
            if (rows > 0)
            {
                program.ActualLevelCount++;
                program.UpdatedAt = DateTime.UtcNow;
                await _programRepository.UpdateAsync(program);
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
        }

        public override async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int ID)
        {
            // Get the level to be deleted
            var level = await _levelRepository.GetByIdAsync(ID);
            if (level == null)
                return (0, new ErrorResponse { Errors = ["Not Found."] });

            var programId = level.ProgramId;

            // Delete the level
            var rows = await _levelRepository.DeleteAsync(level);

            // Decrement ActualLevelCount if deletion was successful
            if (rows > 0)
            {
                var program = await _programRepository.GetByIdAsync(programId);
                if (program != null)
                {
                    program.ActualLevelCount--;
                    program.UpdatedAt = DateTime.UtcNow;
                    await _programRepository.UpdateAsync(program);
                }
            }

            return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
        }
    }
}
