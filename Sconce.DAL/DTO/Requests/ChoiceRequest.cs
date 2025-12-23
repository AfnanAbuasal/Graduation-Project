using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class ChoiceRequest
    {
        [Required] public int QuestionId { get; set; }
        [Required] public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
        [Required] public int SortOrder { get; set; }
    }
}
