using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IGenericService<TRequest, TResponse, TEntity>
    where TEntity : BaseModel
    {
        Task<int> CreateAsync(TRequest request);
        Task<IEnumerable<TResponse>> GetAllAsync(bool onlyActive = false);
        Task<TResponse?> GetByIdAsync(int Id);
        Task<int> DeleteAsync(int Id);
        Task<int> UpdateAsync(int Id, TRequest request);
        Task<bool> ToggleStatusAsync(int Id);
    }
}
