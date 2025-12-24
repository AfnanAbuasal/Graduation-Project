using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Question : BaseModel
    {
        public string Prompt { get; set; }
        public Difficulty Difficulty { get; set; }
        public string CreatedByInstructorId { get; set; }
        public int CourseId { get; set; }
        public Course Course { get; set; }
    }
}
