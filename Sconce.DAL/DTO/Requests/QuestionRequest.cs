using Sconce.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class QuestionRequest
    {
        [Required]
        [EnumDataType(typeof(QuestionType), ErrorMessage = "Question type must be a valid option.")]
        public QuestionType Type { get; set; }
        
        [Required] public string Prompt { get; set; } = string.Empty;
        
        [Required]
        [EnumDataType(typeof(Difficulty), ErrorMessage = "Difficulty must be a valid option.")]
        public Difficulty Difficulty { get; set; }
        
        [Required] public int CourseId { get; set; }
    }
}
