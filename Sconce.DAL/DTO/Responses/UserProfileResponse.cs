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
    public class UserProfileResponse
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? PhoneNumber { get; set; }

        public DateOnly DateOfBirth { get; set; }
        [JsonIgnore] public Gender Gender { get; set; }
        public string GenderDisplay => Gender.ToDisplayString();

        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }

        [JsonIgnore] public UserType UserType { get; set; }
        public string UserTypeDisplay => UserType.ToDisplayString();
    }
}
