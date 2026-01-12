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
    public class ZoomAttendanceRepository : IZoomAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public ZoomAttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> AddAsync(ZoomAttendance entity)
        {
            await _context.Set<ZoomAttendance>().AddAsync(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<ZoomAttendance?> GetByIdAsync(int id)
        {
            return await _context.Set<ZoomAttendance>().FindAsync(id);
        }

        public async Task<IEnumerable<ZoomAttendance>> GetAllAsync(bool withTracking = false)
        {
            var query = _context.Set<ZoomAttendance>().AsQueryable();

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<int> DeleteAsync(ZoomAttendance entity)
        {
            _context.Set<ZoomAttendance>().Remove(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<int> UpdateAsync(ZoomAttendance entity)
        {
            _context.Set<ZoomAttendance>().Update(entity);
            return await _context.SaveChangesAsync();
        }

        public async Task<ZoomAttendance?> GetByZoomMeetingAndStudentAsync(int zoomMeetingId, string studentId, bool withTracking = false)
        {
            var query = _context.Set<ZoomAttendance>()
                .Where(a => a.ZoomMeetingId == zoomMeetingId && a.StudentId == studentId);

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ZoomAttendance>> GetByZoomMeetingIdAsync(int zoomMeetingId, bool withTracking = false)
        {
            var query = _context.Set<ZoomAttendance>()
                .Where(a => a.ZoomMeetingId == zoomMeetingId)
                .Include(a => a.Student)
                .AsQueryable();

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<ZoomAttendance>> GetByStudentIdAsync(string studentId, bool withTracking = false)
        {
            var query = _context.Set<ZoomAttendance>()
                .Where(a => a.StudentId == studentId)
                .Include(a => a.ZoomMeeting)
                .AsQueryable();

            if (!withTracking)
                query = query.AsNoTracking();

            return await query.ToListAsync();
        }
    }
}
