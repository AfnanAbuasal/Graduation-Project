using System.Text.Json.Serialization;

namespace Sconce.DAL.DTO.Responses
{
    public class InstructorProfileResponse : UserProfileResponse
    {
        public int YearsOfExperience { get; set; }
        public bool ExperienceWithTeachingKids { get; set; }

        [JsonIgnore] public string CVPath { get; set; }
        public string? CVUrl { get; set; }
    }
}
