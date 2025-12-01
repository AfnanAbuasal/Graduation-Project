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
    where TResponse : Response
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
            return (number, new Response { Message = $"{number} record(s) created successfully." });
        }

        public async Task<IEnumerable<Response>> GetAllAsync(bool onlyActive = false)
        {
            var entities = await _repository.GetAllAsync();
            if (onlyActive)
                entities = entities.Where(e => e.Status == Status.Active);

            return entities.Adapt<IEnumerable<TResponse>>();
        }

        public async Task<(bool Success, Response Response)> GetByIdAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);

            if (entity == null)
                return (false, new Response { Message = "Entity not found." });

            return (true, entity.Adapt<TResponse>());
        }

        public async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, TRequest request)
        {
            var entity = await _repository.GetByIdAsync(ID);
            if (entity == null)
                return (0, new Response { Message = "Entity not found." });

            request.Adapt(entity);

            var number = await _repository.UpdateAsync(entity);

            return (number, new Response { Message = $"{number} record(s) updated successfully." });
        }

        public async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);

            if (entity == null)
                return (0, new Response { Message = "Entity not found." });

            var number = await _repository.DeleteAsync(entity);

            return (number, new Response { Message = $"{number} record(s) deleted successfully." });
        }

        public async Task<(bool Success, Response Response)> ToggleStatusAsync(int ID)
        {
            var entity = await _repository.GetByIdAsync(ID);

            if (entity == null)
                return (false, new Response { Message = "Entity not found." });

            entity.Status = entity.Status == Status.Active ? Status.Inactive : Status.Active;

            await _repository.UpdateAsync(entity);

            return (true, new Response { Message = "Status toggled successfully." });
        }
    }
}
