using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class DropoutRequest
    {
        public int LevelId { get; set; }
        public string Reasons { get; set; }
    }
}
