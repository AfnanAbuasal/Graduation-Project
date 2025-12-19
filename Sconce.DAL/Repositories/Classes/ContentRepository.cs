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
	public class ContentRepository : GenericRepository<Content>, IContentRepository
	{
		private readonly ApplicationDbContext _context;

		public ContentRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<Content>> GetBySectionIdAsync(int sectionId)
		{
			return await _context.Contents
				.Where(c => c.SectionId == sectionId)
				.OrderBy(c => c.WeekNumber)
				.ThenBy(c => c.CreatedAt)
				.AsNoTracking()
				.ToListAsync();
		}
	}
}
