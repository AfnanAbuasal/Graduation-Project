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
        Task<UserResponse> RegisterStudentAsync(StudentRegisterRequest registerRequest);
        Task<UserResponse> RegisterParentAsync(ParentRegisterRequest request);
        Task<(bool Success, string Message)> ApproveParentLinkAsync(string token);
        Task<UserResponse> RegisterParentWithInviteAsync(ParentRegisterWithInviteRequest request);
        Task<UserResponse> LoginAsync(LoginRequest loginRequest);
        Task<string> ConfirmEmail(string token, string userID);
        Task<string> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest);
        Task<string> ResetPassword(ResetPasswordRequest resetPasswordRequest);
    }
}
