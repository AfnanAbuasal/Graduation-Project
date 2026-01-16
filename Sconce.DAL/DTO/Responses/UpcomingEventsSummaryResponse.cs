using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class UpcomingEventsSummaryResponse
    {
        public int TotalEvents { get; set; }
        public int AssignmentsCount { get; set; }
        public int ExamsCount { get; set; }
        public int ZoomMeetingsCount { get; set; }
    }
}
