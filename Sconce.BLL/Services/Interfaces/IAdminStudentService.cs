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
        Task<IEnumerable<StudentApplicationResponse>> GetAllApplicationsAsync(ApplicationStatus? status = null);
        Task<StudentApplicationResponse?> GetApplicationByIdAsync(int id);
        Task<bool> ReviewApplicationAsync(int id, ApplicationStatus newStatus, string feedback);
    }
}
