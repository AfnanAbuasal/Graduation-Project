using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Student : ApplicationUser
    {
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string DocumentPath { get; set; } // path to the uploaded ID/BirthCertificate file
        public string GuardianName { get; set; }
        public string GuardianEmail { get; set; }
        public LevelOfProficiency LevelOfProficiency { get; set; } = LevelOfProficiency.None;

        // Application tracking
        public DateTime? SubmittedAt { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; } = ApplicationStatus.Pending;
    }
}
