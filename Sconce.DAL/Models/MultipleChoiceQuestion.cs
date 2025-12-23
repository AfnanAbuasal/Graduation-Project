using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class MultipleChoiceQuestion : Question
    {
        public bool AllowMultipleSelections { get; set; } = false;
        public bool ShuffleChoices { get; set; } = true;

        // Navigation property
        public ICollection<Choice> Choices { get; set; }
    }
}
