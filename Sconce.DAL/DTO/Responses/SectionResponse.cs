using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class SectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateTime CreatedAt { get; set; }
        public Status Status { get; set; }
    }
}
