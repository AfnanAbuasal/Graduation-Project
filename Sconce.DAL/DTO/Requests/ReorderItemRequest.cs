using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class ReorderItemRequest
    {
        [Required]
        public int ExamQuestionId { get; set; }

        [Required]
        public int SortOrder { get; set; }
    }
}
