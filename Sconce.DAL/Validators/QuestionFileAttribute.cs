using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Sconce.DAL.Validators
{
    public class QuestionFileAttribute : ValidationAttribute
    {
        private readonly string[] _allowedExtensions = 
        { 
            ".pdf", ".doc", ".docx",                          // Document formats
            ".mp3", ".wav", ".ogg", ".m4a"                   // Audio formats
        };

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
                    return new ValidationResult(ErrorMessage ?? "Please upload a valid file (pdf, doc, docx, or audio files: mp3, wav, ogg, m4a).");
                }
                
                return ValidationResult.Success;
            }
            
            return new ValidationResult("Invalid file type.");
        }
    }
}
