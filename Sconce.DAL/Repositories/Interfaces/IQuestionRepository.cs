using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        Task<IEnumerable<Question>> GetAllByCourseIdAsync(int courseId);
        Task<IEnumerable<Question>> GetByCreatedByInstructorIdAsync(string instructorId);
        Task<Question?> GetByIdWithCourseAsync(int id);
    }
}
