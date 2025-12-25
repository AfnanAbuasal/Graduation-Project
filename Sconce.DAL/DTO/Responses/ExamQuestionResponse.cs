using System;
using System.Text.Json.Serialization;
using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamQuestionResponse
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int SortOrder { get; set; }
        public decimal Points { get; set; }
        public string? PromptOverride { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
