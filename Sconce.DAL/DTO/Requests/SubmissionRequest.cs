using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class SubmissionRequest : IFileRequest
    {
        [Required] public int AssignmentId { get; set; }
        public IFormFile? File { get; set; }
    }
}
