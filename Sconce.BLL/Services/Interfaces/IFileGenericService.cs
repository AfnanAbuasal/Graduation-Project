using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces;

public interface IFileGenericService<TRequest, TResponse, TEntity>
where TEntity : BaseModel, IFileEntity
where TRequest : IFileRequest
{
    Task<(int NumberOfEntries, Response Response)> CreateAsync(TRequest request);
    Task<Response> GetAllAsync(bool onlyActive = false);
    Task<(bool Success, Response Response)> GetByIdAsync(int Id);
    Task<(int NumberOfEntries, Response Response)> DeleteAsync(int Id);
    Task<(int NumberOfEntries, Response Response)> UpdateAsync(int Id, TRequest request);
    Task<(bool Success, Response Response)> ToggleStatusAsync(int Id);
}
