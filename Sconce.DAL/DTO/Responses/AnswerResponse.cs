using System;
using System.Collections.Generic;

namespace Sconce.DAL.DTO.Responses;

public class AnswerResponse
{
    public int Id { get; set; }

    public int ExamAttemptId { get; set; }

    public int ExamQuestionId { get; set; }

    public List<int>? SelectedChoiceIds { get; set; }

    public string? Text { get; set; }

    public string? FileUrl { get; set; }

    public DateTime CreatedAt { get; set; }
}
