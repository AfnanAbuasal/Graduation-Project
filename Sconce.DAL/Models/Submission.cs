using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Submission : BaseModel, IFileEntity
    {
        public string? FilePath { get; set; }
        public string? Feedback { get; set; }
        public decimal? Grade { get; set; }
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public DateTime? GradedAt { get; set; }
        public int AssignmentId { get; set; }
        public Assignment Assignment { get; set; }
        public string StudentId { get; set; }
        public Student Student { get; set; }
    }
}
