using Sconce.DAL.Models.Enums;
using System;

namespace Sconce.DAL.Models
{
    public class ExamAttempt : BaseModel
    {
        public int ExamId { get; set; }
        public Exam Exam { get; set; } = null!;

        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; } = null!;

        public int AttemptNumber { get; set; } = 1;
        public AttemptStatus AttemptStatus { get; set; } = AttemptStatus.InProgress;

        public DateTime StartedAt { get; set; } = DateTime.UtcNow;
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }

        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }

        public DateTime? GradedAt { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
