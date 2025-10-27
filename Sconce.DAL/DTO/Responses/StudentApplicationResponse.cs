using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class StudentApplicationResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }

        public DateOnly DateOfBirth { get; set; }
        [JsonIgnore] public Gender Gender { get; set; }
        public string GenderDisplay => Gender.ToDisplayString();

        public string Country { get; set; }
        public string City { get; set; }
        public string Street { get; set; }

        public string GuardianName { get; set; }
        public string GuardianEmail { get; set; }

        [JsonIgnore] public LevelOfProficiency LevelOfProficiency { get; set; }
        public string LevelOfProficiencyDisplay => LevelOfProficiency.ToDisplayString();

        public DateTime? SubmittedAt { get; set; }
        [JsonIgnore] public ApplicationStatus ApplicationStatus { get; set; }
        public string ApplicationStatusDisplay => ApplicationStatus.ToDisplayString();

        public string Message { get; set; }
    }
}
