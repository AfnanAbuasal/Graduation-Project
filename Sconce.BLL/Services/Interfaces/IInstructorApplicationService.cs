using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IInstructorApplicationService
    {
        Task<(bool Success, Response Response)> SubmitApplicationAsync(InstructorApplicationRequest request);
        Task<(bool Success, Response Response)> GetApplicationByEmailAsync(string email);
    }
}
