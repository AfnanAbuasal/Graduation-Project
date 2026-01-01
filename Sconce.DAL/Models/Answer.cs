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

    /// Grading: Awarded score (null if not graded yet).
    public decimal? Score { get; set; }

    /// Grading: Maximum possible score (snapshot of ExamQuestion.Points).
    public decimal MaxScore { get; set; }

    /// Grading: When this answer was graded.
    public DateTime? GradedAt { get; set; }

    /// Grading: Instructor who graded this answer.
    public string? GradedByInstructorId { get; set; }
}
