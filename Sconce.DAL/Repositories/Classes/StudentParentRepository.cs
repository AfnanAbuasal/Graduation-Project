using Microsoft.EntityFrameworkCore;
using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Classes
{
    public class StudentParentRepository : IStudentParentRepository
    {
        private readonly ApplicationDbContext _context;

        public StudentParentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(StudentParent relation)
        {
            _context.StudentParents.Add(relation);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<StudentParent>> GetByStudentIdAsync(string studentId)
        {
            return await _context.StudentParents
                .Where(sp => sp.StudentId == studentId)
                .Include(sp => sp.Parent)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentParent>> GetByParentIdAsync(string parentId)
        {
            return await _context.StudentParents
                .Where(sp => sp.ParentId == parentId)
                .Include(sp => sp.Student)
                .ToListAsync();
        }
    }
}
