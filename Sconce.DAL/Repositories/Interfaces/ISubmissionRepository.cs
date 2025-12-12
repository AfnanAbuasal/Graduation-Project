using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface ISubmissionRepository : IGenericRepository<Submission>
    {
        Task<IEnumerable<Submission>> GetAllWithStudentAsync();
        Task<Submission?> GetByIdWithStudentAsync(int id);
        Task<Submission?> GetByAssignmentAndStudentAsync(int assignmentId, string studentId);
    }
}
