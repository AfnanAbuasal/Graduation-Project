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
    public class LevelRepository : GenericRepository<Level>, ILevelRepository
    {
        private readonly ApplicationDbContext _context;

        public LevelRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Level>> GetAllWithProgramAsync()
        {
            return await _context.Levels
                .Include(l => l.Program)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Level?> GetByIdWithProgramAsync(int id)
        {
            return await _context.Levels
                .Include(l => l.Program)
                .AsNoTracking()
                .FirstOrDefaultAsync(l => l.Id == id);
        }
    }
}
