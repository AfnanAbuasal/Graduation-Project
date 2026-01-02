using System.Collections.Generic;

namespace Sconce.DAL.DTO.Responses
{
    /// <summary>
    /// Full details of an exam attempt including all questions and student answers.
    /// </summary>
    public class ExamAttemptDetailsResponse : Response
    {
        /// <summary>
        /// Basic attempt information including all student answers.
        /// </summary>
        public ExamAttemptResponse Attempt { get; set; } = null!;

        /// <summary>
        /// List of all exam questions with their details.
        /// </summary>
        public List<ExamQuestionDetailsResponse> Questions { get; set; } = new();
    }
}