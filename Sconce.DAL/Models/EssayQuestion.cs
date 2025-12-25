using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class EssayQuestion : Question
    {
        public bool AllowFileUpload { get; set; } = false;
        public int? MaxWords { get; set; }
        public int? MaxFileSizeMb { get; set; }
        public string? QuestionFilePath { get; set; }
    }
}
