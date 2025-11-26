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
        Task<Response> RegisterStudentAsync(StudentRegisterRequest registerRequest);
        Task<Response> RegisterParentAsync(ParentRegisterRequest request);
        Task<(bool Success, Response Response)> ApproveParentLinkAsync(string token);
        Task<Response> RegisterParentWithInviteAsync(ParentRegisterWithInviteRequest request);
        Task<Response> LoginAsync(LoginRequest loginRequest);
        Task<Response> ConfirmEmailAsync(string token, string userID);
        Task<Response> ForgotPasswordAsync(ForgotPasswordRequest forgotPasswordRequest);
        Task<Response> ResetPasswordAsync(ResetPasswordRequest resetPasswordRequest);
    }
}
