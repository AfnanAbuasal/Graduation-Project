using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class InstructorApplicationResponse
    {
        public Guid ApplicationId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string? Street { get; set; }
        public int YearsOfExperience { get; set; }
        public bool ExperienceWithTeachingKids { get; set; }
        public DateTime? SubmittedAt { get; set; } = DateTime.UtcNow;
        [JsonIgnore] public ApplicationStatus ApplicationStatus { get; set; }
        public string ApplicationStatusDisplay => ApplicationStatus.ToDisplayString();

        //public string CVPath { get; set; } // optional if you want to show the uploaded file link
        public string Feedback { get; set; } = "Your application has been submitted successfully. Please wait while the manager reviews it.";
    }
}
