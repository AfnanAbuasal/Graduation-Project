using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IStudentParentRepository
    {
        Task AddAsync(StudentParent relation);
        Task<IEnumerable<StudentParent>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<StudentParent>> GetByParentIdAsync(string parentId);
    }
}
