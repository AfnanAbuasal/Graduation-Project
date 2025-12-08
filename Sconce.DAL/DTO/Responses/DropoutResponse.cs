using Sconce.DAL.Models.Enums;
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
        public string Reasons { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public int ProgramId { get; set; }
        public string ProgramName { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
    }
}
