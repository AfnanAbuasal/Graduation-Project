using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
	public class ContentResponse
	{
		public int Id { get; set; }
		public int SectionId { get; set; }
		public int WeekNumber { get; set; }
		public string Type { get; set; } = string.Empty;
		public string Title { get; set; } = string.Empty;
		public string? Description { get; set; }
		public string? Url { get; set; }
		public ZoomDataResponse? ZoomData { get; set; }
		public DateTime CreatedAt { get; set; }
		public string? InstructorId { get; set; }
	}
}
