using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class SubmissionRepository : GenericRepository<Submission>, ISubmissionRepository
    {
        private readonly ApplicationDbContext _context;

        public SubmissionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Submission>> GetAllWithStudentAsync()
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Submission?> GetByIdWithStudentAsync(int id)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Submission?> GetByAssignmentAndStudentAsync(int assignmentId, string studentId)
        {
            return await _context.Submissions
                .Include(s => s.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.AssignmentId == assignmentId && s.StudentId == studentId);
        }

        public async Task<IEnumerable<Submission>> GetAllByAssignmentIdAsync(int assignmentId, bool withTracking = false)
        {
            var query = _context.Submissions
                .Where(s => s.AssignmentId == assignmentId)
                .Include(s => s.Student);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
