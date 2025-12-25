using Microsoft.AspNetCore.Http;
using Sconce.DAL.Validators;
using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class EssayQuestionRequest : QuestionRequest, IFileRequest
    {
        public bool AllowFileUpload { get; set; } = false;
        [Range(1, int.MaxValue, ErrorMessage = "MaxWords must be greater than 0 when provided.")]
        public int? MaxWords { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "MaxFileSizeMb must be greater than 0 when provided.")]
        public int? MaxFileSizeMb { get; set; }
        [QuestionFile(ErrorMessage = "Please upload a valid file (pdf, doc, docx, or audio files: mp3, m4a, wav, ogg).")]
        public IFormFile? File { get; set; }
    }
}
