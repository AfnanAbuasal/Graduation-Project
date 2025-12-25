using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class IncreaseCapacityRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Additional capacity must be greater than 0.")]
        public int AdditionalCapacity { get; set; }
    }
}
