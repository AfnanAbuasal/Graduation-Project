using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IStudentApplicationService
    {
        Task<(bool Success, Response Response)> SubmitApplicationAsync(StudentApplicationRequest request);
        Task<(bool Success, Response Response)> GetApplicationByEmailAsync(string email);
    }
}
