namespace Sconce.DAL.DTO.Responses
{
    public class ChoiceResponse
    {
        public int QuestionId { get; set; }
        public string Text { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
