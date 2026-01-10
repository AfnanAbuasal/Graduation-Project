using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamStatusResponse
    {
        public int Id { get; set; }
        [JsonIgnore] public ExamStatus ExamStatus { get; set; }
        public string ExamStatusDisplay => ExamStatus.ToDisplayString();
    }
}
