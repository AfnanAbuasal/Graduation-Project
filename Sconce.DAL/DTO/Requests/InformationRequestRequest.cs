using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class InformationRequestRequest
    {
        [Required]
        [StringLength(50, ErrorMessage = "Role cannot exceed 50 characters.")]
        public string Role { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Full Name cannot exceed 50 characters.")]
        public string FullName { get; set; } = string.Empty;

        [Required]
        [StringLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string Country { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Email format is invalid.")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters.")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Phone number format is invalid.")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters.")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(2000, ErrorMessage = "Question cannot exceed 2000 characters.")]
        public string Question { get; set; } = string.Empty;
    }
}
