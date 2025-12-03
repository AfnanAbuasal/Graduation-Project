using Mapster;
using Sconce.BLL.Services.Interfaces;
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
    public class GenericService<TRequest, TResponse, TEntity> : IGenericService<TRequest, TResponse, TEntity>
    where TEntity : BaseModel
    {
        private readonly IGenericRepository<TEntity> _repository;

        public GenericService(IGenericRepository<TEntity> repository)
        {
            _repository = repository;
        }

        public virtual async Task<(int NumberOfEntries, Response Response)> CreateAsync(TRequest request)
        {
            var entity = request.Adapt<TEntity>();
            var number = await _repository.AddAsync(entity);
            return (number, new SuccessResponse<string> { Data = $"{number} record(s) created successfully." });
        }

        public async Task<Response> GetAllAsync(bool onlyActive = false)
        {
            var entities = await _repository.GetAllAsync();
            if (onlyActive)
                entities = entities.Where(e => e.Status == Status.Active);

            return new SuccessResponse<IEnumerable<TResponse>> { Data = entities.Adapt<IEnumerable<TResponse>>() };
        }

        public async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);

            if (entity == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Not Found." } });

            return (true, new SuccessResponse<TResponse> { Data = entity.Adapt<TResponse>() });
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, TRequest request)
        {
            var entity = await _repository.GetByIdAsync(ID);
            if (entity == null)
                return (0, new ErrorResponse { Errors = new List<string> { "Not Found." } });

            request.Adapt(entity);

            var number = await _repository.UpdateAsync(entity);

            return (number, new SuccessResponse<string> { Data = $"{number} record(s) updated successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);

            if (entity == null)
                return (0, new ErrorResponse { Errors = new List<string> { "Not Found." } });

            var number = await _repository.DeleteAsync(entity);

            return (number, new SuccessResponse<string> { Data = $"{number} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> ToggleStatusAsync(int ID)
        {
            var entity = await _repository.GetByIdAsync(ID);

            if (entity == null)
                return (false, new ErrorResponse { Errors = new List<string> { "Not Found." } });

            entity.Status = entity.Status == Status.Active ? Status.Inactive : Status.Active;

            await _repository.UpdateAsync(entity);

            return (true, new SuccessResponse<string> { Data = "Status toggled successfully." });
        }
    }
}
