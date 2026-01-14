using System;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamPerformanceSummaryResponse
    {
        public int TotalExams { get; set; }
        public int TotalAttempts { get; set; }
        public int CompletedCount { get; set; } // Graded attempts
        public int AttemptedCount { get; set; } // Submitted/Expired but not graded
        public int MissingAttemptsCount { get; set; } // Exams not attempted
        public decimal AverageScorePercentage { get; set; }
    }
}
