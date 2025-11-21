using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Extensions
{
    public static class EnumExtensions
    {
        // Status
        public static string ToDisplayString(this Status status)
        {
            return status switch
            {
                Status.Active => "Active",
                Status.Inactive => "Inactive",
                _ => "Unknown"
            };
        }

        // Level of Proficiency
        public static string ToDisplayString(this LevelOfProficiency level)
        {
            return level switch
            {
                LevelOfProficiency.None => "New / No prior knowledge",
                LevelOfProficiency.A1 => "Beginner (A1)",
                LevelOfProficiency.A2 => "Elementary (A2)",
                LevelOfProficiency.B1 => "Intermediate (B1)",
                LevelOfProficiency.B2 => "Upper Intermediate (B2)",
                LevelOfProficiency.C1 => "Advanced (C1)",
                LevelOfProficiency.C2 => "Proficient (C2)",
                _ => "Unknown"
            };
        }

        // Application Status
        public static string ToDisplayString(this ApplicationStatus status)
        {
            return status switch
            {
                ApplicationStatus.Pending => "Pending Review",
                ApplicationStatus.Approved => "Approved",
                ApplicationStatus.Rejected => "Rejected",
                _ => "Unknown"
            };
        }

        // Gender
        public static string ToDisplayString(this Gender gender)
        {
            return gender switch
            {
                Gender.Unspecified => "Unspecified",
                Gender.Male => "Male",
                Gender.Female => "Female",
                _ => "Unknown"
            };
        }
    }
}
