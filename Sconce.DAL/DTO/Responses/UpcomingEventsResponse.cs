using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class UpcomingEventsResponse
    {
        public IEnumerable<UpcomingEventItemResponse> Events { get; set; }
        public UpcomingEventsSummaryResponse Summary { get; set; }
    }
}
