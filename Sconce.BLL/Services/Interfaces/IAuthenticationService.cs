using Sconce.DAL.DTO.Requests;
using Sconce.DAL.DTO.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface IAuthenticationService
    {
        Task<(bool Success, Response Response)> RegisterStudentAsync(StudentRegisterRequest registerRequest);
        Task<(bool Success, Response Response)> RegisterParentAsync(ParentRegisterRequest request);
        Task<(bool Success, Response Response)> ApproveParentLinkAsync(string token);
        Task<(bool Success, Response Response)> RegisterParentWithInviteAsync(ParentRegisterWithInviteRequest request);
        Task<(bool Success, Response Response)> LoginAsync(LoginRequest loginRequest);
        Task<(bool Success, Response Response)> ConfirmEmailAsync(string token, string userID);
        Task<(bool Success, Response Response)> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
        Task<(bool Success, Response Response)> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }
}
