using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class ProgramRequest
    {
        [Required] public string Name { get; set; }
        public string? Description { get; set; }
        [Required] public DateTime StartDate { get; set; }
        [Required] public DateTime EndDate { get; set; }
        [Required] public int PlannedCourseCount { get; set; }
    }
}
