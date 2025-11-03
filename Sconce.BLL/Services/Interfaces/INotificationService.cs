using Sconce.DAL.DTO.Requests;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sconce.BLL.Services.Interfaces
{
    public interface INotificationService
    {
        Task SendApplicationSubmittedAsync(InstructorApplication app);
        Task SendApplicationApprovedAsync(InstructorApplication app, string password, string emailConfirmationURL);
        Task SendApplicationRejectedAsync(InstructorApplication app);
        Task SendConfirmEmailAsync(ApplicationUser user, string emailConfirmationURL);
        Task SendPasswordResetCodeAsync(ForgotPasswordRequest forgotPasswordRequest, string code);
        Task SendPasswordResetSuccessAsync(ResetPasswordRequest resetPasswordRequest, ApplicationUser user);
    }
}
