using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using Sconce.DAL.Models;
using System;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamAttemptResponse
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentFullName { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        [JsonIgnore] public AttemptStatus AttemptStatus { get; set; }
        public string AttemptStatusDisplay => AttemptStatus.ToDisplayString();
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
        public DateTime? GradedAt { get; set; }
        public ICollection<AnswerResponse> Answers { get; set; }
    }
}
