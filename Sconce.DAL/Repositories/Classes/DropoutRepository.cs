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
    public class DropoutRepository : GenericRepository<Dropout>, IDropoutRepository
    {
        private readonly ApplicationDbContext _context;

        public DropoutRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Dropout?> GetByIdWithStudentAsync(int id)
        {
            return await _context.Dropouts
                .Include(d => d.Student)
                .Include(d => d.Level)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }
    }
}
