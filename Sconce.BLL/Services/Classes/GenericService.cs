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

namespace Sconce.BLL.Services.Classes;

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
        var rows = await _repository.AddAsync(entity);
        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) created successfully." });
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
            return (false, new ErrorResponse { Errors = ["Not Found."] });

        return (true, new SuccessResponse<TResponse> { Data = entity.Adapt<TResponse>() });
    }

    public virtual async Task<(int NumberOfEntries, Response Response)> UpdateAsync(int ID, TRequest request)
    {
        var entity = await _repository.GetByIdAsync(ID);
        if (entity == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        request.Adapt(entity);

        entity.UpdatedAt = DateTime.UtcNow;

        var rows = await _repository.UpdateAsync(entity);

        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) updated successfully." });
    }

    public virtual async Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id)
    {
        var entity = await _repository.GetByIdAsync(Id);

        if (entity == null)
            return (0, new ErrorResponse { Errors = ["Not Found."] });

        var rows = await _repository.DeleteAsync(entity);

        return (rows, new SuccessResponse<string> { Data = $"{rows} record(s) deleted successfully." });
    }

    public async Task<(bool Success, Response Response)> ToggleStatusAsync(int ID)
    {
        var entity = await _repository.GetByIdAsync(ID);

        if (entity == null)
            return (false, new ErrorResponse { Errors = ["Not Found."] });

        entity.Status = entity.Status == Status.Active ? Status.Inactive : Status.Active;

        entity.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(entity);

        return (true, new SuccessResponse<string> { Data = "Status toggled successfully." });
    }
}
