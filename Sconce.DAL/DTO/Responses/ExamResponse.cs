using System;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }
        public int? DurationMinutes { get; set; }
        public int AttemptsAllowed { get; set; }
        public bool ShuffleQuestions { get; set; }
        public int SectionId { get; set; }
        public int WeekNumber { get; set; }
		public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public ExamStatus ExamStatus { get; set; }
    }
}
