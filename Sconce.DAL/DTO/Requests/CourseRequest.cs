using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class CourseRequest
    {
        [Required] public string Name { get; set; }
        public string? Description { get; set; }
        [Required] public int ProgramId { get; set; }
    }
}
