using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Document : Content, IFileEntity
    {
        public string Title { get; set; } = string.Empty;
        public string? FilePath { get; set; } = string.Empty;
    }
}
