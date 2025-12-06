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
        public string DocumentPath { get; set; } // path to the uploaded ID/BirthCertificate file
        public LevelOfProficiency LevelOfProficiency { get; set; } = LevelOfProficiency.None;
        public ReferralSource ReferralSource { get; set; } = ReferralSource.Other;
        public ICollection<StudentParent> StudentParents { get; set; } = new List<StudentParent>();
    }
}
