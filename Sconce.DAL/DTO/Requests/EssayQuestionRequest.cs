using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class EssayQuestionRequest : QuestionRequest
    {
        public bool AllowFileUpload { get; set; } = false;
        [Range(1, int.MaxValue, ErrorMessage = "MaxWords must be greater than 0 when provided.")]
        public int? MaxWords { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "MaxFileSizeMb must be greater than 0 when provided.")]
        public int? MaxFileSizeMb { get; set; }
    }
}
