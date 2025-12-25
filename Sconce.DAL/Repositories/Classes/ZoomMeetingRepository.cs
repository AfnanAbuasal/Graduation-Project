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
	public class ZoomMeetingRepository : GenericRepository<ZoomMeeting>, IZoomMeetingRepository
	{
		private readonly ApplicationDbContext _context;

		public ZoomMeetingRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public async Task<IEnumerable<ZoomMeeting>> GetAllBySectionIdAsync(int sectionId, bool withTracking = false)
		{
			var query = _context.Set<ZoomMeeting>().Where(z => z.SectionId == sectionId);

			if (!withTracking)
				query = query.AsNoTracking();

			return await query.ToListAsync();
		}
	}
}
