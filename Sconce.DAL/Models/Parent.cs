using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Parent : ApplicationUser
    {
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
}
