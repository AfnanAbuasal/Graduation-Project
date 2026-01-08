using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class RecommendCourseRequest
    {
        [Required]
        public int RecommendedCourseId { get; set; }
    }
}