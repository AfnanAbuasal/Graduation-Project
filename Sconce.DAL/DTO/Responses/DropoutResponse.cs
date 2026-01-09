using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class DropoutResponse
    {
        public int Id { get; set; }
        public string Reasons { get; set; } = string.Empty;
        [JsonIgnore] public ApplicationStatus ApplicationStatus { get; set; }
        public string ApplicationStatusDisplay => ApplicationStatus.ToDisplayString();
        public DateTime CreatedAt { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
    }
}
