using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamQuestionDetailsResponse
    {
        public int Id { get; set; }
        public int ExamId { get; set; }
        public int QuestionId { get; set; }
        public int SortOrder { get; set; }
        public decimal Points { get; set; }
        public string Prompt { get; set; }
        [JsonIgnore] public Difficulty Difficulty { get; set; }
        public string DifficultyDisplay => Difficulty.ToDisplayString();
        public List<ChoiceResponse> Choices { get; set; }
    }
}
