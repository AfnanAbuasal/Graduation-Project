using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class StudentApplication : BaseModel
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string? Street { get; set; }
        public string DocumentPath { get; set; }
        public string? GuardianName { get; set; }
        public string? GuardianEmail { get; set; }
        public LevelOfProficiency LevelOfProficiency { get; set; } = LevelOfProficiency.None;

        // Application tracking
        public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Pending;
        public string? Feedback { get; set; }
    }
}
