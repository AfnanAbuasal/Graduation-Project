using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Requests
{
    public class StudentRegisterRequest
    {
        [Required] public string Email { get; set; }
        [Required] public string FullName { get; set; }
        [Required] public string Password { get; set; }
        [Required]
        [EnumDataType(typeof(ReferralSource))]
        [Range((int)ReferralSource.Search, (int)ReferralSource.Other, ErrorMessage = "Referral source must be a valid option.")]
        public ReferralSource ReferralSource { get; set; } = ReferralSource.Other;
    }
}
