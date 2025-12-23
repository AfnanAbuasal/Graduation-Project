using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sconce.DAL.Validators
{
    public class PdfFileAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (extension != ".pdf")
                {
                    return new ValidationResult(ErrorMessage ?? "Please upload a valid pdf file.");
                }
                
                return ValidationResult.Success;
            }
            
            return new ValidationResult("Invalid file type.");
        }
    }
}
