using System.Collections.Generic;

namespace Sconce.DAL.DTO.Responses
{
    public class MultipleChoiceQuestionResponse : QuestionResponse
    {
        public bool AllowMultipleSelections { get; set; }
        public bool ShuffleChoices { get; set; }
        public ICollection<ChoiceResponse> Choices { get; set; } = new List<ChoiceResponse>();
    }
}
