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
        public int ProgramId { get; set; }
        public Program Program { get; set; }
        //public ICollection<Section> Sections { get; set; } = new List<Section>();
    }
}
