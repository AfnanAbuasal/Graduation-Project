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
    public class SubmissionRequest : IFileRequest
    {
        [Required] public int AssignmentId { get; set; }
        
        [DocumentFile(ErrorMessage = "Please upload a valid document file (pdf, doc, docx, or txt).")]
        public IFormFile? File { get; set; }
    }
}
