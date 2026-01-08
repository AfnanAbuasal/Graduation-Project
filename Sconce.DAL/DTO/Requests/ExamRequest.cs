using System;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class ExamRequest
    {
        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        public int? SectionId { get; set; }
        public int? ProgramId { get; set; }

        [Required]
        public int WeekNumber { get; set; }

        public DateTime? AvailableFrom { get; set; }
        public DateTime? AvailableTo { get; set; }

        public int? DurationMinutes { get; set; }
        public int AttemptsAllowed { get; set; } = 1;
        public bool ShuffleQuestions { get; set; } = false;
    }
}
