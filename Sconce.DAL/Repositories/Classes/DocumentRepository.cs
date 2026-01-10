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
    public class DocumentRepository : GenericRepository<Document>, IDocumentRepository
    {
        private readonly ApplicationDbContext _context;

        public DocumentRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Document>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false)
        {
            IQueryable<Document> query = _context.Set<Document>()
                .Where(d => d.SectionId == sectionId)
                .OrderBy(d => d.WeekNumber)
                .ThenBy(d => d.CreatedAt);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
