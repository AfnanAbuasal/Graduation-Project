using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class StudentProfileResponse : UserProfileResponse
    {
        [JsonIgnore] public LevelOfProficiency LevelOfProficiency { get; set; }
        public string LevelOfProficiencyDisplay => LevelOfProficiency.ToDisplayString();

        [JsonIgnore] public string DocumentPath { get; set; }
        public string? DocumentUrl { get; set; }
    }
}
