using Microsoft.AspNetCore.Http;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace Sconce.DAL.Utilities
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class PdfFileAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            // If no file provided, pass validation (property is optional)
            if (file is null)
            {
                return ValidationResult.Success;
            }

            var extension = Path.GetExtension(file.FileName);
            if (!string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult("Only PDF files are allowed.");
            }

            // Enforce content type where available
            if (!string.Equals(file.ContentType, "application/pdf", StringComparison.OrdinalIgnoreCase))
            {
                return new ValidationResult("Only PDF files are allowed.");
            }

            return ValidationResult.Success;
        }
    }
}