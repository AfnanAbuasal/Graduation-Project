using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class PerformanceFilterRequest
    {
        // Time window in days (e.g., 7, 30). If null, defaults to section start date.
        public int? WindowDays { get; set; }

        [Required]
        public int SectionId { get; set; }

        [Required]
        public string StudentId { get; set; }
    }
}
