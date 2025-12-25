using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class ExamQuestionRequest
    {
        [Required] public int ExamId { get; set; }

        [Required] public int QuestionId { get; set; }

        [Required] public int SortOrder { get; set; }

        [Required] public decimal Points { get; set; }

        public string? PromptOverride { get; set; }
    }
}
