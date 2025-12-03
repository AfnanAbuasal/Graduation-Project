using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.DAL.DTO.Responses
{
    public class SuccessResponse<T> : Response
    {
        public T Data { get; set; }
    }
}
