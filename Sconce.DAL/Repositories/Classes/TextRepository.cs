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
    public class TextRepository : GenericRepository<Text>, ITextRepository
    {
        private readonly ApplicationDbContext _context;

        public TextRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Text>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false)
        {
            var query = _context.Set<Text>().Where(t => t.SectionId == sectionId);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
