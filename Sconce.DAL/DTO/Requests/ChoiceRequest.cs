using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class ChoiceRequest
    {
        [Required] public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; } = false;
    }
}
