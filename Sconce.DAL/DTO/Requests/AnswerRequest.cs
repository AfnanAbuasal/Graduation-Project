using Microsoft.AspNetCore.Http;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.Validators;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests;

public class AnswerRequest : IFileRequest
{
    [Required] public int ExamAttemptId { get; set; }

    [Required] public int ExamQuestionId { get; set; }

    public List<int>? SelectedChoiceIds { get; set; }

    public string? Text { get; set; }

    [QuestionFile(ErrorMessage = "Please upload a valid file (pdf, doc, docx, or audio files: mp3, m4a, wav, ogg).")]
    public IFormFile? File { get; set; }
}
