using System;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamPerformanceItemResponse
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }
        public int AttemptId { get; set; }
        public int AttemptNumber { get; set; }
        public string Status { get; set; } // "Submitted", "Expired", "Graded"
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        public decimal? ScorePercentage { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? GradedAt { get; set; }
    }
}
