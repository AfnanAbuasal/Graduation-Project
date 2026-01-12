using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class AssignmentPerformanceItemResponse
    {
        public int AssignmentId { get; set; }
        public string Title { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? GradedAt { get; set; }
        public decimal? Grade { get; set; }
        public string Status { get; set; } // "Submitted" or "Missing"
    }
}
