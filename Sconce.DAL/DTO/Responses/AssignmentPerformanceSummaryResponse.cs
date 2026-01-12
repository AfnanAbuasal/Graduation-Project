using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class AssignmentPerformanceSummaryResponse
    {
        public int TotalAssignments { get; set; }
        public int SubmittedCount { get; set; }
        public int MissingCount { get; set; }
        public int GradedCount { get; set; }
        public decimal AverageGrade { get; set; }
        public int SubmittedOnTimeCount { get; set; }
    }
}
