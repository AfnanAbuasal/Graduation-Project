using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ProgramEnrollmentResponse
    {
        public int Id { get; set; }
        
        public int ProgramId { get; set; }
        public string? ProgramName { get; set; }
        
        public string StudentId { get; set; } = string.Empty;
        public string? StudentFullName { get; set; }
        public int? StudentAge { get; set; }
        
        public DateTime CreatedAt { get; set; }

        public string? ProficiencyExamStatusDisplay { get; set; }

        public decimal? ExamScore { get; set; }
        public decimal? ExamMaxScore { get; set; }
        public decimal? ExamScorePercentage { get; set; }

        public int? RecommendedCourseId { get; set; }
        public string? RecommendedCourseName { get; set; }

        public int? PlacedSectionId { get; set; }
        public string? PlacedSectionName { get; set; }

        public string? EvaluatedByInstructorName { get; set; }
        public DateTime? EvaluatedAt { get; set; }
    }
}
