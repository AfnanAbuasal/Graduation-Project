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
        public ApplicationStatus ApplicationStatus { get; set; }
        public string Feedback { get; set; }
    }
}
