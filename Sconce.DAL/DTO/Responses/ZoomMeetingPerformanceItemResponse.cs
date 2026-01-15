using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Extensions;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomMeetingPerformanceItemResponse
    {
        public int ZoomMeetingId { get; set; }
        public string Title { get; set; }
        public DateTime ScheduledTime { get; set; }
        [JsonIgnore] public AttendanceStatus? Attended { get; set; }
        public string? AttendanceStatusDisplay => Attended?.ToDisplayString();
        public DateTime? RecordedAt { get; set; }
    }
}
