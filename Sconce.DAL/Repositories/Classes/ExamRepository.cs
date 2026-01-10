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
    public class ExamRepository : GenericRepository<Exam>, IExamRepository
    {
        private readonly ApplicationDbContext _context;

        public ExamRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public override async Task<Exam?> GetByIdAsync(int id)
        {
            var query = _context.Set<Exam>()
            .Include(e => e.Section)
            .ThenInclude(s => s.Course)
            .Where(e => e.Id == id);

            return await query.FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<Exam>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false)
        {
            var query = _context.Set<Exam>()
            .Include(e => e.Section)
            .ThenInclude(s => s.Course)
            .Where(e => e.SectionId == sectionId);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<Exam>> GetAllByProgramIdAsync(int programId, bool withTracking = false)
        {
            var query = _context.Set<Exam>()
                .Where(e => e.ProgramId == programId);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
