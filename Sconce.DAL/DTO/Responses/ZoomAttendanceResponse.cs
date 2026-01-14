using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ZoomAttendanceResponse
    {
        public int Id { get; set; }
        public int ZoomMeetingId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        [JsonIgnore] public AttendanceStatus Attended { get; set; }
        public string AttendanceStatusDisplay => Attended.ToDisplayString();
        public DateTime RecordedAt { get; set; }
    }
}
