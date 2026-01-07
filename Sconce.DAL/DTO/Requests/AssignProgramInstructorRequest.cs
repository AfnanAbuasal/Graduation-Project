using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class AssignProgramInstructorRequest
    {
        [Required]
        public string InstructorId { get; set; }
    }
}
