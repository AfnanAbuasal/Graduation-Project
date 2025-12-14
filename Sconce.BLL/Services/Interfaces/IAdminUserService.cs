using Sconce.DAL.DTO.Responses;
using Sconce.DAL.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IAdminUserService
    {
        Task<Response> GetAllUserProfilesAsync(UserType? userType = null);
        Task<(bool Success, Response Response)> GetUserProfileByIdAsync(string userId);
    }
}
