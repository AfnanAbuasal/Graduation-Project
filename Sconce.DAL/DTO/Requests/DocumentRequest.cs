using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class DocumentRequest : IFileRequest
    {
        public string Title { get; set; } = string.Empty;
        public IFormFile? File { get; set; } = default!;
        public int SectionId { get; set; }
        public int WeekNumber { get; set; }
    }
}
