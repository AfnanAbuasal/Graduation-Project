using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Requests
{
    public class InstructorApplicationRequest
    {
        [Required] public string FullName { get; set; }
        [Required, EmailAddress] public string Email { get; set; }
        [Required] public string PhoneNumber { get; set; }
        [Required] public DateOnly DateOfBirth { get; set; }
        [Required] public Gender Gender { get; set; }
        [Required] public string Country { get; set; }
        [Required] public string City { get; set; }
        public string Street { get; set; }
        [Required] public int YearsOfExperience { get; set; }
        public bool ExperienceWithTeachingKids { get; set; } = false;
        [Required] public IFormFile CV { get; set; }
    }
}
