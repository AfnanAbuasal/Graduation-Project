using System;
using System.Text.Json.Serialization;
using Sconce.DAL.Extensions;
using Sconce.DAL.Models.Enums;

namespace Sconce.DAL.DTO.Responses
{
    public class ExamQuestionDetailsResponse
    {
        // Has-A relationships (composition)
        public ExamQuestionResponse ExamQuestion { get; set; }
        public QuestionResponse Question { get; set; }
    }
}
