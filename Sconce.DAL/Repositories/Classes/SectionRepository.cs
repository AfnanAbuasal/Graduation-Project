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
    public class SectionRepository : GenericRepository<Section>, ISectionRepository
    {
        private readonly ApplicationDbContext _context;

        public SectionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Section?> GetByIdWithInstructorAsync(int id)
        {
            return await _context.Sections
                .Include(s => s.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<IEnumerable<Section>> GetAllWithInstructorAsync()
        {
            return await _context.Sections
                .Include(s => s.Instructor)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
