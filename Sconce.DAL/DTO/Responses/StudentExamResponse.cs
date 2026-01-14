using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Responses
{
    public class StudentExamResponse : ExamResponse
    {
        public bool Attempted { get; set; }
        public int RemainingAttempts { get; set; }
    }
}
