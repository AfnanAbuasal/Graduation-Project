using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class ParentRegisterRequest
    {
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public DateOnly DateOfBirth { get; set; }
        [Required] public Gender Gender { get; set; }
        [Required] public string RelationshipWithStudent { get; set; }
        [Required] public string Password { get; set; }
        [Required, EmailAddress] public string StudentEmail { get; set; }
    }
}
