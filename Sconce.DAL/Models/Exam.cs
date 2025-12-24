using Sconce.DAL.Models.Enums;
using System;

namespace Sconce.DAL.Models
{
    public class Exam : Content
    {
        public string Title { get; set; } = string.Empty;

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public int? DurationMinutes { get; set; }
        public int AttemptsAllowed { get; set; } = 1;

        public bool ShuffleQuestions { get; set; } = false;

        public ExamStatus ExamStatus { get; set; } = ExamStatus.Draft;
    }
}
