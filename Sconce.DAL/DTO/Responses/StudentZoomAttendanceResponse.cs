using Sconce.DAL.Models.Enums;
using Sconce.DAL.Extensions;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class StudentZoomAttendanceResponse
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        [JsonIgnore] public AttendanceStatus? AttendanceStatus { get; set; }
        public string? AttendanceStatusDisplay => AttendanceStatus?.ToDisplayString();
        public DateTime? RecordedAt { get; set; }
    }
}
