using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class ResetPasswordRequest
    {
        [Required] public string Email { get; set; }
        [Required] public string NewPassword { get; set; }
        [Required] public string Code { get; set; }
    }
}
