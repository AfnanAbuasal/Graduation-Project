using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomAttendanceResponse
    {
        public int Id { get; set; }
        public int ZoomMeetingId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public bool Attended { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
