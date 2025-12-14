using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Antiforgery;

namespace Sconce.DAL.DTO.Requests
{
    public class AssignInstructorRequest
    {
        [Required] public string InstructorId { get; set; }
    }
}