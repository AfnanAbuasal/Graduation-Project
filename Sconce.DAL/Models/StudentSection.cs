using System;

namespace Sconce.DAL.Models
{
    public class StudentSection
    {
        public string StudentId { get; set; } = string.Empty;
        public Student Student { get; set; }

        public int SectionId { get; set; }
        public Section Section { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    }
}