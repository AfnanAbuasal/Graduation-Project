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
        public string? Description { get; set; }
        public int PlannedLevelCount { get; set; }
        public int ActualLevelCount { get; set; } = 0;
        public ICollection<Level> Levels { get; set; } = new List<Level>();
    }
}
