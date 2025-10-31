using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file, string folder);
        void DeleteFile(string relativePath);
    }
}
