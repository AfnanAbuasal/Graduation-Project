using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Program : BaseModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int PlannedLevelCount { get; set; }
        public int ActualLevelCount { get; set; } = 0;
        public bool HasProficiencyExam { get; set; } = false;
        public int? ProficiencyExamId { get; set; }
        public Exam? ProficiencyExam { get; set; }

        public string? ExamWriterInstructorId { get; set; }
        public Instructor? ExamWriterInstructor { get; set; }

        public string? EvaluatorInstructorId { get; set; }
        public Instructor? EvaluatorInstructor { get; set; }

        public ICollection<Level> Levels { get; set; } = new List<Level>();
        public ICollection<ProgramEnrollment> Enrollments { get; set; } = new List<ProgramEnrollment>();
    }
}
