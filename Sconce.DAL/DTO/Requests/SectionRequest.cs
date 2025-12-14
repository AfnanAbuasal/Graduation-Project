using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class SectionRequest
    {
        [Required] public string Name { get; set; }
        public string? Description { get; set; }
        [Required] public int Capacity { get; set; }
        [Required] public int CourseId { get; set; }
        public string? InstructorId { get; set; }
    }
}
