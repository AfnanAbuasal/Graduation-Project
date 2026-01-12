using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomMeetingPerformanceResponse
    {
        public IEnumerable<ZoomMeetingPerformanceItemResponse> ZoomMeetings { get; set; }
        public ZoomMeetingPerformanceSummaryResponse Summary { get; set; }
    }
}
