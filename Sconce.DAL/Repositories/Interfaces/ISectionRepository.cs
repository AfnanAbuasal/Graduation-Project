using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface ISectionRepository : IGenericRepository<Section>
    {
        Task<Section?> GetByIdWithInstructorAsync(int id);
        Task<Section?> GetByIdWithCourseAsync(int id);
        Task<IEnumerable<Section>> GetAllWithInstructorAsync();
        Task<IEnumerable<Section>> GetByInstructorIdWithInstructorAsync(string instructorId);
        Task<IEnumerable<Section>> GetByCourseIdAsync(int courseId, bool onlyActive = false);
    }
}
