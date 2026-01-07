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
        [Required] public int PlannedLevelCount { get; set; }
        public bool HasProficiencyExam { get; set; } = false;
        public string? ExamWriterInstructorId { get; set; }
        public string? EvaluatorInstructorId { get; set; }
    }
}
