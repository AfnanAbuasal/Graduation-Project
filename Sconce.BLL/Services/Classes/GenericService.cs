using Mapster;
using MapsterMapper;
using Sconce.BLL.Services.Interfaces;
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
        public async Task<int> CreateAsync(TRequest request)
        {
            var entity = request.Adapt<TEntity>();
            return await _repository.AddAsync(entity);
        }

        public async Task<int> DeleteAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);
            if (entity == null) return 0;
            return await _repository.DeleteAsync(entity);
        }

        public async Task<IEnumerable<TResponse>> GetAllAsync(bool onlyActive = false)
        {
            var entities = await _repository.GetAllAsync();
            if (onlyActive)
                entities = entities.Where(e => e.Status == Status.Active);

            return entities.Adapt<IEnumerable<TResponse>>();
        }

        public async Task<TResponse?> GetByIdAsync(int Id)
        {
            var entity = await _repository.GetByIdAsync(Id);
            return entity == null ? default : entity.Adapt<TResponse>();
        }

        public async Task<bool> ToggleStatusAsync(int ID)
        {
            var entity = await _repository.GetByIdAsync(ID);
            if (entity == null) return false;

            entity.Status = entity.Status == Status.Active ? Status.Inactive : Status.Active;
            await _repository.UpdateAsync(entity);
            return true;
        }

        public async Task<int> UpdateAsync(int ID, TRequest request)
        {
            var entity = await _repository.GetByIdAsync(ID);
            if (entity == null) return 0;

            request.Adapt(entity);
            return await _repository.UpdateAsync(entity);
        }
    }
}
