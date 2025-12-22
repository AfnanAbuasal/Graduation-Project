using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Dropout : BaseModel
    {
        public string Reasons { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public string StudentId { get; set; }
        public Student Student { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Pending;
    }
}
