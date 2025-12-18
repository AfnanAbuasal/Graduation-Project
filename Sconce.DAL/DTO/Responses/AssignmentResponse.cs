using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class AssignmentResponse
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public DateTime DueDate { get; set; }
        public decimal MinGrade { get; set; }
        public decimal MaxGrade { get; set; }
        public int SectionId { get; set; }
        public int WeekNumber { get; set; }
        public string? FileUrl { get; set; }
    }
}
