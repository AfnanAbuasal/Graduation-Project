using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class InformationRequest : BaseModel
    {
        public string Role { get; set; } = string.Empty;   // "I am a" (Student/Parent/Instructor/Other)
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Question { get; set; } = string.Empty;
    }
}
