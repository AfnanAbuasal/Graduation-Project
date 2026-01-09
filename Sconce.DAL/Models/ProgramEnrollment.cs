using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class ProgramEnrollment : BaseModel
    {
        public int ProgramId { get; set; }
        public Program Program { get; set; }

        public string StudentId { get; set; }
        public Student Student { get; set; }

        public int? StudentAge { get; set; }

        public int? ProficiencyExamAttemptId { get; set; }
        public ExamAttempt? ProficiencyExamAttempt { get; set; }

        public int? RecommendedCourseId { get; set; }
        public Course? RecommendedCourse { get; set; }

        public string? EvaluatedByInstructorId { get; set; }
        public Instructor? EvaluatedByInstructor { get; set; }

        public int? PlacedSectionId { get; set; }
        public Section? PlacedSection { get; set; }

        public DateTime? EvaluatedAt { get; set; }
    }
}
