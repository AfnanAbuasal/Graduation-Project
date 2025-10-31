using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IGenericRepository<T> where T : BaseModel
    {
        Task<int> AddAsync(T entity);
        Task<T?> GetByIdAsync(int Id);
        Task<IEnumerable<T>> GetAllAsync(bool withTracking = false);
        Task<int> DeleteAsync(T entity);
        Task<int> UpdateAsync(T entity);
    }
}
