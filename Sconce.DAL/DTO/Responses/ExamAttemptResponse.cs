using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamAttemptResponse : Response
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int AttemptNumber { get; set; }
        [JsonIgnore] public AttemptStatus Status { get; set; }
        public string AttemptStatusDisplay => Status.ToDisplayString();
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public DateTime? ExpiresAt { get; set; }
        public decimal? Score { get; set; }
        public decimal? MaxScore { get; set; }
    }
}
