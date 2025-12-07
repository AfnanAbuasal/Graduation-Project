using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Assignment : BaseModel, IFileEntity
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal MinGrade { get; set; }
        public decimal MaxGrade { get; set; }
        public string? FilePath { get; set; }
        public int SectionId { get; set; }
        public Section Section { get; set; }
        public ICollection<Submission> Submissions { get; set; } = new List<Submission>();
    }
}
