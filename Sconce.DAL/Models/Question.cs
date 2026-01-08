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
        public string Prompt { get; set; } = string.Empty;
        public Difficulty Difficulty { get; set; }
        public string CreatedByInstructorId { get; set; } = string.Empty;

        // Normal flow (Course)
        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        // Proficiency flow (Program)
        public int? ProgramId { get; set; }
        public Program? Program { get; set; }

        public string Type { get; private set; }

        public Question()
        {
            // Set to most-derived class name (e.g., EssayQuestion)
            Type = GetType().Name;
        }
    }
}
