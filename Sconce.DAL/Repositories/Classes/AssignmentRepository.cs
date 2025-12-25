using Sconce.DAL.Data;
using Sconce.DAL.Models;
using Sconce.DAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Sconce.DAL.Repositories.Classes
{
    public class AssignmentRepository : GenericRepository<Assignment>, IAssignmentRepository
    {
        private readonly ApplicationDbContext _context;

        public AssignmentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Assignment>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false)
        {
            var query = _context.Set<Assignment>().Where(a => a.SectionId == sectionId);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
