using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomMeetingPerformanceSummaryResponse
    {
        public int TotalMeetings { get; set; }
        public int AttendedCount { get; set; }
        public int MissedCount { get; set; }
        public int AttendanceNotMarkedCount { get; set; }
        public decimal AttendanceRate { get; set; }
    }
}
