using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class InformationRequestResponse
    {
        public int Id { get; set; }
        public string Role { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }
        public string Question { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
