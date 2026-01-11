using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface ILevelRepository : IGenericRepository<Level>
    {
        Task<IEnumerable<Level>> GetAllWithProgramAsync();
        Task<IEnumerable<Level>> GetAllByProgramAsync(int programId, bool onlyActive = false);
        Task<IEnumerable<Level>> GetAllByProgramWithCoursesAsync(int programId);
        Task<Level?> GetByIdWithProgramAsync(int id);
    }
}
