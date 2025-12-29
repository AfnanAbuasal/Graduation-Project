using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class SubmitExamAttemptRequest
    {
        [Required]
        public int AttemptId { get; set; }
    }
}
