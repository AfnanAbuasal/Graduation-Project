using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class AssignmentPerformanceResponse
    {
        public IEnumerable<AssignmentPerformanceItemResponse> Assignments { get; set; }
        public AssignmentPerformanceSummaryResponse Summary { get; set; }
    }
}
