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
    public class ProgramResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public int PlannedLevelCount { get; set; }
        public int ActualLevelCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool HasProficiencyExam { get; set; }
        public int? ProficiencyExamId { get; set; }
        public string? ExamWriterInstructorId { get; set; }
        public string? EvaluatorInstructorId { get; set; }
        [JsonIgnore] public Status Status { get; set; }
        public string StatusDisplay => Status.ToDisplayString();
    }
}
