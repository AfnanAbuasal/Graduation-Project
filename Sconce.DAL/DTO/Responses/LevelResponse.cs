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
    public class LevelResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public int PlannedCourseCount { get; set; }
        public int ActualCourseCount { get; set; }
        public int? PrerequisiteLevelId { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore] public Status Status { get; set; }
        public string StatusDisplay => Status.ToDisplayString();
    }
}
