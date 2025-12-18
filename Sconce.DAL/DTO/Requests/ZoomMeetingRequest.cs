using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
	public class ZoomMeetingRequest
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Url { get; set; } = string.Empty;
		public int SectionId { get; set; }
		public int WeekNumber { get; set; }
		public ZoomDataRequest ZoomData { get; set; } = new();
	}

	public class ZoomDataRequest
	{
		public string MeetingId { get; set; } = string.Empty;
		public string? Password { get; set; }
		public DateTime StartTime { get; set; }
		public int Duration { get; set; }
		public ZoomSettingsRequest? Settings { get; set; }
	}

	public class ZoomSettingsRequest
	{
		public bool WaitingRoom { get; set; }
		public bool MuteUponEntry { get; set; }
	}
}
