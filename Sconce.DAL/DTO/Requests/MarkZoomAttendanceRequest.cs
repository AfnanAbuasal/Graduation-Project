using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class MarkZoomAttendanceRequest
    {
        [Required]
        public int ZoomMeetingId { get; set; }

        [Required]
        public string StudentId { get; set; }

        [Required]
        public bool Attended { get; set; }
    }
}
