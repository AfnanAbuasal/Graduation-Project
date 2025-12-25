using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.Models;

public class ExamQuestion : BaseModel
{
    public int ExamId { get; set; }
    public Exam Exam { get; set; }

    public int QuestionId { get; set; }
    public Question Question { get; set; }

    public int SortOrder { get; set; }
    public decimal Points { get; set; }
    public string? PromptOverride { get; set; }
}
