using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class DropoutRequest
    {
        [Required] public int ProgramId { get; set; }
        [Required] public string Reasons { get; set; } = string.Empty;
    }
}
