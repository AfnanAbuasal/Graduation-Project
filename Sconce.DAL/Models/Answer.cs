namespace Sconce.DAL.Models;

public class Answer : BaseModel, IFileEntity
{
    public int ExamAttemptId { get; set; }
    public ExamAttempt ExamAttempt { get; set; } = null!;

    public int ExamQuestionId { get; set; }
    public ExamQuestion ExamQuestion { get; set; } = null!;

    /// For Multiple Choice Questions: JSON array of selected choice IDs.
    /// Example: "[1,5,9]"
    public string? SelectedChoiceIdsJson { get; set; }

    /// For Essay Questions: Text answer provided by student.
    public string? Text { get; set; }

    /// For Essay Questions: Optional file upload path.
    public string? FilePath { get; set; }
}
