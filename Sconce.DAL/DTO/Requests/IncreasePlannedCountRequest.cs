using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Requests
{
    public class IncreasePlannedCountRequest
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Increment must be at least 1.")]
        public int Increment { get; set; }
    }
}
