using System.Collections.Generic;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamPerformanceResponse
    {
        public IEnumerable<ExamPerformanceItemResponse> ExamAttempts { get; set; }
        public ExamPerformanceSummaryResponse Summary { get; set; }
    }
}
