using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class ParentLink : BaseModel
    {
        public string Token { get; set; }

        [Required]
        public string ParentId { get; set; }
        public Parent Parent { get; set; }

        [Required, EmailAddress]
        public string StudentEmail { get; set; }

        public bool IsApproved { get; set; } = false;
        public bool IsUsed { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
