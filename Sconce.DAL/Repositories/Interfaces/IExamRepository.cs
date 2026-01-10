using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IExamRepository : IGenericRepository<Exam>
    {
        Task<IEnumerable<Exam>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false);
        Task<IEnumerable<Exam>> GetAllByProgramIdAsync(int programId, bool withTracking = false);
    }
}
