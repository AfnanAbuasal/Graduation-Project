using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomMeetingPerformanceItemResponse
    {
        public int ZoomMeetingId { get; set; }
        public string Title { get; set; }
        public DateTime ScheduledTime { get; set; }
        public bool? Attended { get; set; }
        public DateTime? RecordedAt { get; set; }
    }
}
