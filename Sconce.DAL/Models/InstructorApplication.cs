using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class InstructorApplication : BaseModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public int YearsOfExperience { get; set; }
        public bool ExperienceWithTeachingKids { get; set; }
        public string CVPath { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Pending;
        public string Feedback { get; set; }
    }
}
