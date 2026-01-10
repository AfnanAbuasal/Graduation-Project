using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class DocumentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public int SectionId { get; set; }
        public int WeekNumber { get; set; }
        public string Type { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
