using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }

        // Password Reset
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiration { get; set; }
    }
}
