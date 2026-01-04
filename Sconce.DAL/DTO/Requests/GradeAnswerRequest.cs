using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests;

public class GradeAnswerRequest
{
    [Required]
    public decimal Score { get; set; }
}
