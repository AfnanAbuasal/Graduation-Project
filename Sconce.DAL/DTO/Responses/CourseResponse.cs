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
    public class CourseResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int Order { get; set; }
        public int LevelId { get; set; }
        public string? LevelName { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore] public Status Status { get; set; }
        public string StatusDisplay => Status.ToDisplayString();
    }
}
