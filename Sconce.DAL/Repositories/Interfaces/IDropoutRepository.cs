using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IDropoutRepository : IGenericRepository<Dropout>
    {
        Task<Dropout?> GetByIdWithStudentAndProgramAsync(int id);
        Task<IEnumerable<Dropout>> GetAllWithStudentAndProgramAsync();
        Task<IEnumerable<Dropout>> GetByProgramWithStudentAndProgramAsync(int programId);
        Task<Dropout?> GetByProgramAndStudentAsync(int programId, string studentId);
    }
}
