using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class StudentParent
    {
        public string StudentId { get; set; } = string.Empty;
        public string ParentId { get; set; } = string.Empty;

        public Student Student { get; set; } = null!;
        public Parent Parent { get; set; } = null!;

        public string RelationshipWithStudent { get; set; } // Father / Mother / Guardian
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;
        public bool IsConfirmed { get; set; } = false;
    }
}
