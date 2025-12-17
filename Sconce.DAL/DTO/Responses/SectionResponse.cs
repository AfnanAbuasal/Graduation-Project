using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class SectionResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Capacity { get; set; }
        public int CourseId { get; set; }
        public string? CourseName { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string? InstructorId { get; set; }
        public string? InstructorName { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore] public Status Status { get; set; }
        public string StatusDisplay => Status.ToDisplayString();
    }
}
