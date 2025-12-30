using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface ICourseRepository : IGenericRepository<Course>
    {
        Task<IEnumerable<Course>> GetAllWithLevelAsync();
        Task<Course?> GetByIdWithLevelAsync(int id);
        Task<IEnumerable<Course>> GetByLevelIdAsync(int levelId, bool onlyActive = false);
    }
}
