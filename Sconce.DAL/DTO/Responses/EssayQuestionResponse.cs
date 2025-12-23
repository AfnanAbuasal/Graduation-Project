namespace Sconce.DAL.DTO.Responses
{
    public class EssayQuestionResponse : QuestionResponse
    {
        public bool AllowFileUpload { get; set; }
        public int? MaxWords { get; set; }
        public int? MaxFileSizeMb { get; set; }
    }
}
