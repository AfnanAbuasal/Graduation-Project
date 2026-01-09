using Sconce.DAL.Models;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IStudentSectionRepository
    {
        Task<bool> ExistsAsync(string studentId, int sectionId);
        Task<int> GetCountBySectionIdAsync(int sectionId);
        Task<int> AddAsync(StudentSection entity);
        Task<IEnumerable<StudentSection>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Student>> GetStudentsBySectionIdAsync(int sectionId);
    }
}
