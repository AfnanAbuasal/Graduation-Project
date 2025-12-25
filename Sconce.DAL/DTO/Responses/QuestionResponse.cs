using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class QuestionResponse
    {
        public int Id { get; set; }
        public string Prompt { get; set; } = string.Empty;
        [JsonIgnore] public Difficulty Difficulty { get; set; }
        public string DifficultyDisplay => Difficulty.ToDisplayString();
        public string CreatedByInstructorId { get; set; } = string.Empty;
        public int CourseId { get; set; }
		public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        [JsonIgnore] public Status Status { get; set; }
        public string StatusDisplay => Status.ToDisplayString();
    }
}
