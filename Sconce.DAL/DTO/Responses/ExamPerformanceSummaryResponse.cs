using System;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamPerformanceSummaryResponse
    {
        public int TotalExams { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedCount { get; set; } // Graded attempts
        public int AttemptedCount { get; set; } // Submitted/Expired but not graded
        public decimal AverageScorePercentage { get; set; }
    }
}
