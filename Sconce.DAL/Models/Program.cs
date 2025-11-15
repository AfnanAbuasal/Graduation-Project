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
        //public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
