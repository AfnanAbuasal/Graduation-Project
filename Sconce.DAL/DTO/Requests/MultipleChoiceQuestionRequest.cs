using System.ComponentModel.DataAnnotations;

namespace Sconce.DAL.DTO.Requests
{
    public class MultipleChoiceQuestionRequest : QuestionRequest
    {
        public bool AllowMultipleSelections { get; set; } = false;
        public bool ShuffleChoices { get; set; } = true;
    }
}
