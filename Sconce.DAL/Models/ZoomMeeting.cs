using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
	public class ZoomMeeting : Content
	{
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string Url { get; set; } = string.Empty;
		public ZoomData? ZoomData { get; set; }
	}

	[Owned]
	public class ZoomData
	{
		public string MeetingId { get; set; } = string.Empty;
		public string? Password { get; set; }
		public DateTime StartTime { get; set; }
		public int Duration { get; set; }

		public ZoomSettings? Settings { get; set; }
	}

	[Owned]
	public class ZoomSettings
	{
		public bool JoinBeforeHost { get; set; }
		public bool WaitingRoom { get; set; }
		public bool MuteUponEntry { get; set; }
	}
}
