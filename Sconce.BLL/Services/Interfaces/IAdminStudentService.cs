using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IAdminStudentService
    {
        Task<Response> GetAllApplicationsAsync(ApplicationStatus? status = null);
        Task<(bool Success, Response Response)> GetApplicationByIdAsync(int id);
        Task<(bool Success, Response Response)> ReviewApplicationAsync(int id, ApplicationStatus newStatus, string feedback);
    }
}
