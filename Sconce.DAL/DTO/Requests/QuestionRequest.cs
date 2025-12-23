using Sconce.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class QuestionRequest
    {
        [Required] public QuestionType Type { get; set; }
        [Required] public string Prompt { get; set; } = string.Empty;
        [Required] public Difficulty Difficulty { get; set; }
        [Required] public int CourseId { get; set; }
    }
}
