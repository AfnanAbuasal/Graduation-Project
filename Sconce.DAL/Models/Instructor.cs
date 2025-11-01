using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Instructor : ApplicationUser
    {
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public int YearsOfExperience { get; set; }
        public bool ExperienceWithTeachingKids { get; set; } = false;
        public string CVPath { get; set; } // path to uploaded CV
    }
}
