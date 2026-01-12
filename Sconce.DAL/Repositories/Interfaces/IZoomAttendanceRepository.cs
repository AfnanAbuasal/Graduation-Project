using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Repositories.Interfaces
{
    public interface IZoomAttendanceRepository
    {
        Task<int> AddAsync(ZoomAttendance entity);
        Task<ZoomAttendance?> GetByIdAsync(int id);
        Task<IEnumerable<ZoomAttendance>> GetAllAsync(bool withTracking = false);
        Task<int> DeleteAsync(ZoomAttendance entity);
        Task<int> UpdateAsync(ZoomAttendance entity);
        Task<ZoomAttendance?> GetByZoomMeetingAndStudentAsync(int zoomMeetingId, string studentId, bool withTracking = false);
        Task<IEnumerable<ZoomAttendance>> GetByZoomMeetingIdAsync(int zoomMeetingId, bool withTracking = false);
        Task<IEnumerable<ZoomAttendance>> GetByStudentIdAsync(string studentId, bool withTracking = false);
    }
}
