using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class ParentInvite
    {
        [Key]
        public string Token { get; set; } = Guid.NewGuid().ToString("N"); // unique token

        [Required]
        public string StudentId { get; set; }
        public Student Student { get; set; }  // optional navigation property

        [Required]
        [EmailAddress]
        public string GuardianEmail { get; set; }

        public DateTime ExpiresAt { get; set; } = DateTime.UtcNow.AddDays(3); // 3-day validity
        public bool IsUsed { get; set; } = false;
    }
}
