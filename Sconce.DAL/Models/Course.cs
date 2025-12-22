using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Course : BaseModel
    {
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Order { get; set; }
        public int LevelId { get; set; }
        public Level Level { get; set; }
        public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
