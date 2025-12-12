using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Section : BaseModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
        public string? InstructorId { get; set; }
        public Instructor? Instructor { get; set; }
    }
}
