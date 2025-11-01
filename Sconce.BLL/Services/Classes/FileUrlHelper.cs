using Microsoft.AspNetCore.Http;
using Sconce.BLL.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Classes
{
    public class FileUrlHelper : IFileUrlHelper
    {
        private readonly IHttpContextAccessor _accessor;

        public FileUrlHelper(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        public string? BuildFileUrl(string? relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;

            var request = _accessor.HttpContext?.Request;
            if (request == null) return relativePath; // fallback

            var baseUrl = $"{request.Scheme}://{request.Host}";
            return $"{baseUrl}{relativePath}";
        }
    }
}
