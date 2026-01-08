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
        Task SendApplicationSubmittedAsync(StudentApplication app);
        Task SendApplicationApprovedAsync(StudentApplication app);
        Task SendApplicationRejectedAsync(StudentApplication app);
        Task SendParentLinkRequestAsync(Parent parent, Student student, string relationship, string approvalUrl);
        Task SendParentLinkedAsync(Student student, Parent parent, string relationship);
        Task SendStudentLinkedAsync(Parent parent, Student student, string emailConfirmationURL);
        Task SendParentInvitationAsync(StudentApplication app, string invitationLink);
        Task SendConfirmEmailAsync(ApplicationUser user, string emailConfirmationURL);
        Task SendPasswordResetCodeAsync(ForgotPasswordRequest forgotPasswordRequest, string code);
        Task SendPasswordResetSuccessAsync(ResetPasswordRequest resetPasswordRequest, ApplicationUser user);
        Task SendSubmissionCreatedAsync(Submission submission, Assignment assignment);
        Task SendSubmissionUpdatedAsync(Submission submission, Assignment assignment);
        Task SendSubmissionDeletedAsync(Submission submission, Assignment assignment);
        Task SendSubmissionGradedAsync(Submission submission, Assignment assignment);
        Task SendDropoutRequestedAsync(Dropout dropout);
        Task SendDropoutUpdatedAsync(Dropout dropout);
        Task SendDropoutCancelledAsync(Dropout dropout);
        Task SendDropoutApprovedAsync(Dropout dropout);
        Task SendDropoutRejectedAsync(Dropout dropout, string feedback);
        Task SendExamWriterAssignedAsync(Instructor instructor, string programName);
        Task SendEvaluatorAssignedAsync(Instructor instructor, string programName);
        Task SendLoginNotificationAsync(ApplicationUser user, DateTime loginTime);
        Task SendExamAttemptSubmittedAsync(Student student, string examTitle, int attemptNumber, DateTime submittedAt);
        Task SendExamAttemptExpiredAsync(Student student, string examTitle, int attemptNumber, DateTime expiredAt);
        Task SendExamAttemptGradedAsync(Student student, string examTitle, int attemptNumber, decimal score, decimal maxScore);
    }
}
