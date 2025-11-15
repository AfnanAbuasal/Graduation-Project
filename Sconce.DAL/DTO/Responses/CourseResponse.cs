using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class CourseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int ProgramId { get; set; }
        public string? ProgramName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
    }
}
