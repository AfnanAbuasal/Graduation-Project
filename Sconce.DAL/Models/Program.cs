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
        public string Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PlannedCourseCount { get; set; }
        public int ActualCourseCount { get; set; } = 0;
        public int? PrerequisiteProgramId { get; set; }
        public Program PrerequisiteProgram { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
