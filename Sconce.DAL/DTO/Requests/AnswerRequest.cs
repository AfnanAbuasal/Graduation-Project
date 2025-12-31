using Microsoft.AspNetCore.Http;
using Sconce.DAL.DTO.Requests;
using System.Collections.Generic;

namespace Sconce.DAL.DTO.Requests;

public class AnswerRequest : IFileRequest
{
    public int ExamAttemptId { get; set; }

    public int ExamQuestionId { get; set; }

    public List<int>? SelectedChoiceIds { get; set; }

    public string? Text { get; set; }

    public IFormFile? File { get; set; }
}
