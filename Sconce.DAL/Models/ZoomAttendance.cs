using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
	public class ZoomAttendance
	{
        public int Id { get; set; }
		public int ZoomMeetingId { get; set; }
		public ZoomMeeting ZoomMeeting { get; set; }
		public string StudentId { get; set; }
		public Student Student { get; set; }
		public bool Attended { get; set; }
		public DateTime RecordedAt { get; set; }
	}
}
