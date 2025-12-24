using System.ComponentModel.DataAnnotations;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class ApplicationReviewRequest
    {
        [Required]
        [EnumDataType(typeof(ApplicationStatus), ErrorMessage = "Application status must be a valid option.")]
        public ApplicationStatus ApplicationStatus { get; set; }

        [Required] public string Feedback { get; set; }
    }
}
