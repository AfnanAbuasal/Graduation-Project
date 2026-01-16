using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class UpcomingEventItemResponse
    {
        public string Type { get; set; } // "Assignment", "Exam", "ZoomMeeting"
        public string Title { get; set; }
        public string SectionName { get; set; }
        public DateTime EventDate { get; set; }
        public int DueInDays { get; set; } // -1 for open exams, calculated days for others
        public bool Submitted { get; set; }
        public string? Time { get; set; } // For Zoom meetings (e.g., "8:10 PM")
    }
}
