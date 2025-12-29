using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class StartExamAttemptRequest
    {
        [Required]
        public int ExamId { get; set; }
    }
}
