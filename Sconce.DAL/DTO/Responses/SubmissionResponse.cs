using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class SubmissionResponse
    {
        public int Id { get; set; }
        public string? FileUrl { get; set; }
        public string? Feedback { get; set; }
        public decimal? Grade { get; set; }
        public DateTime SubmittedAt { get; set; }
        public DateTime? GradedAt { get; set; }
        public int AssignmentId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
    }
}
