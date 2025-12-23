using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sconce.DAL.Validators;

namespace Sconce.DAL.DTO.Requests
{
    public class AssignmentRequest : IFileRequest
    {
        [Required] public string Title { get; set; }
        public string? Description { get; set; }
        [Required] public DateTime DueDate { get; set; }
        [Required] public decimal MinGrade { get; set; }
        [Required] public decimal MaxGrade { get; set; }
        [Required] public int SectionId { get; set; }
        [Required] public int WeekNumber { get; set; }

        [DocumentFile(ErrorMessage = "Please upload a valid document file (pdf, doc, docx, or txt).")]
        public IFormFile? File { get; set; }
    }
}
