using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
	public class ZoomMeetingResponse
	{
		public int Id { get; set; }
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Url { get; set; } = string.Empty;
		public int SectionId { get; set; }
		public int WeekNumber { get; set; }
		public string Type { get; set; } = string.Empty;
		public string InstructorId { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public ZoomDataResponse? ZoomData { get; set; }
	}

	public class ZoomDataResponse
	{
		public string MeetingId { get; set; } = string.Empty;
		public string? Password { get; set; }
		public DateTime StartTime { get; set; }
		public int Duration { get; set; }
		public ZoomSettingsResponse? Settings { get; set; }
	}

	public class ZoomSettingsResponse
	{
		public bool WaitingRoom { get; set; }
		public bool MuteUponEntry { get; set; }
	}
}
