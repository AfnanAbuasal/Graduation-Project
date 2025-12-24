using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.Models
{
    public class Text : Content
    {
        public string? Title { get; set; }
        public string? Body { get; set; }
    }
}
