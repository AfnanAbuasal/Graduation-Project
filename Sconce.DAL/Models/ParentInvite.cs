using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class ParentInvite : BaseModel
    {
        public string Token { get; set; }

        [Required]
        public string StudentId { get; set; }
        public Student Student { get; set; }

        [Required]
        [EmailAddress]
        public string GuardianEmail { get; set; }

        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
