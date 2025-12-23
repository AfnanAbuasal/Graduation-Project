using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sconce.DAL.Validators
{
    public class DocumentFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = { ".pdf", ".doc", ".docx", ".txt" };

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            if (value is IFormFile file)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                
                if (!_allowedExtensions.Contains(extension))
                {
                    return new ValidationResult(ErrorMessage ?? "Please upload a valid document file (pdf, doc, docx, or txt).");
                }
                
                return ValidationResult.Success;
            }
            
            return new ValidationResult("Invalid file type.");
        }
    }
}
