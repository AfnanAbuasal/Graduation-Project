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

        public LevelService(ILevelRepository levelRepository) : base(levelRepository)
        {
            _levelRepository = levelRepository;
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
    }
}
