using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IDbService
    {
        Task<bool> DeleteUserByEmail(string email);
    }
}
