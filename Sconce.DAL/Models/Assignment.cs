using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Assignment : Content, IFileEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime DueDate { get; set; }
        public decimal MinGrade { get; set; }
        public decimal MaxGrade { get; set; }
        public string? FilePath { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
