using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class TextRequest : IValidatableObject
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
        [Required] public int SectionId { get; set; }
        [Required] public int WeekNumber { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Body))
            {
                yield return new ValidationResult(
                    "Provide either a title or a body.",
                    new[] { nameof(Title), nameof(Body) }
                );
            }
        }
    }
}
