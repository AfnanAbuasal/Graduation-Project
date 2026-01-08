using Sconce.DAL.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class QuestionRequest
    {
        [Required] public string Prompt { get; set; } = string.Empty;
        
        [Required]
        [EnumDataType(typeof(Difficulty), ErrorMessage = "Difficulty must be a valid option.")]
        public Difficulty Difficulty { get; set; }
        
        public int? CourseId { get; set; }
        public int? ProgramId { get; set; }
    }
}
