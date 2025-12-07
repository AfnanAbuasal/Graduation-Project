using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class GradeSubmissionRequest
    {
        public decimal Grade { get; set; }
        public string? Feedback { get; set; }
    }
}
