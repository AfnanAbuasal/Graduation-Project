using Microsoft.AspNetCore.Identity.UI.Services;
using Sconce.BLL.Services.Interfaces;
using Sconce.DAL.DTO.Requests;
using Sconce.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Sconce.BLL.Services.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;

        public NotificationService(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        // Instructor Emails
        public async Task SendApplicationSubmittedAsync(InstructorApplication app)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "Sconce Application Submitted",
                $@"
                <h2>Hello {app.FullName},</h2>
                <p>Thank you for applying to join Sconce as an instructor!</p>
                <p>Your application has been received and is currently under review by our team.</p>
                <p>We’ll notify you by email once a decision has been made.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendApplicationApprovedAsync(InstructorApplication app, string password, string emailConfirmationURL)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "🎉 Your Instructor Application Has Been Approved!",
                $@"
                <h2>Congratulations, {app.FullName}!</h2>
                <p>We’re excited to let you know that your application to join Sconce has been <b>approved</b>.</p>
                <p>To activate your account, please confirm your email by clicking the button below:</p>
                <a href='{emailConfirmationURL}' 
                   style='background-color:#58ACAA;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;'>Confirm Email</a>
                <p>Once confirmed, you’ll be able to log in using:</p>
                <ul>
                  <li><b>Email:</b> {app.Email}</li>
                  <li><b>Password:</b> {password}</li>
                </ul>
                <p style='color:red;'>Please log in and change your password as soon as possible for security.</p>
                <br/>
                <p>Welcome aboard! 🌟</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendApplicationRejectedAsync(InstructorApplication app)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "Your Instructor Application Result",
                $@"
                <h2>Hello {app.FullName},</h2>
                <p>We appreciate your interest in joining Sconce as an instructor.</p>
                <p>Unfortunately, after careful review, your application was not approved at this time.</p>
                <br/>
                <p><b>Feedback:</b> {app.Feedback}</p>
                <br/>
                <p>You’re always welcome to reapply in the future. 💛</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        // Student Emails
        public async Task SendApplicationSubmittedAsync(StudentApplication app)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "Sconce Application Submitted",
                $@"
                <h2>Hello {app.FullName},</h2>
                <p>Thank you for applying to join <b>Sconce</b> as a student! 🎓</p>
                <p>Your application has been successfully received and is currently under review by our admissions team.</p>
                <p>We’ll notify you via email once your application has been processed.</p>
                <br/>
                <p>We’re excited to have you begin your learning journey with us!</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendApplicationApprovedAsync(StudentApplication app)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "🎉 Your Student Application Has Been Approved!",
                $@"
                <h2>Congratulations, {app.FullName}!</h2>
                <p>We’re excited to let you know that your application to join <b>Sconce</b> as a student has been <b>approved</b> 🎓.</p>

                <p>You can now log in to your account and proceed to payment and proficiency exam.</p>
                <p>We’re thrilled to have you join our learning community and can’t wait to see what you’ll achieve!</p>

                <br/>
                <p>Welcome to Sconce 🌟</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendApplicationRejectedAsync(StudentApplication app)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "Sconce Application Update",
                $@"
                <h2>Hello {app.FullName},</h2>
                <p>Thank you for your interest in joining <b>Sconce</b>.</p>
                <p>After carefully reviewing your student application, we regret to inform you that it was <b>not approved</b> at this time.</p>
                <p>We encourage you to reapply in the future once you’ve updated your information or met the necessary requirements.</p>
                <br/>
                <p><b>Feedback:</b> {app.Feedback}</p>
                <br/>
                <p>We truly appreciate your effort and wish you all the best in your educational journey!</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendParentLinkRequestAsync(Parent parent, Student student, string relationship, string approvalUrl)
        {
            await _emailSender.SendEmailAsync(
                student.Email,
                "Parent Link Request — Sconce",
                $@"
                <h2>Parent Link Request</h2>
                <p>Your {relationship}, <b>{parent.FullName}</b> (<i>{parent.Email}</i>), 
                wants to link their account to yours on Sconce.</p>
                <p>If you recognize this request, click the button below to approve:</p>
                <a href='{approvalUrl}' 
                   style='background-color:#58ACAA;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;'>Approve</a>
                <br/><br/>
                <p>If you don’t recognize this person, ignore this message.</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendParentLinkedAsync(Student student, Parent parent, string relationship)
        {
            await _emailSender.SendEmailAsync(
                student.Email,
                $"👨‍👩‍👧 Your {relationship} Account Has Been Linked",
                $@"
                <h2>Hello {student.FullName},</h2>
                <p>We wanted to let you know that your Sconce account has been successfully linked to your {relationship}:</p>
                <ul>
                    <li><b>{relationship} Name:</b> {parent.FullName}</li>
                    <li><b>Email:</b> {parent.Email}</li>
                </ul>
                <p>This allows your {relationship} to stay updated on your learning progress and manage related settings securely.</p>
                <p>If you believe this link was made in error, please contact our support team immediately.</p>
                <br/>
                <p>Stay curious and keep learning! 🌟</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }


        // Parent Emails
        public async Task SendParentInvitationAsync(StudentApplication app, string invitationUrl)
        {
            var guardianName = string.IsNullOrEmpty(app.GuardianName) ? "Parent/Guardian" : app.GuardianName;

            await _emailSender.SendEmailAsync(
                app.GuardianEmail!,
                "Invitation to Join Sconce as a Guardian",
                $@"
                <h2>Hello {guardianName},</h2>
                <p>Your child <b>{app.FullName}</b> has been accepted into the Sconce learning platform. 🎉</p>
                <p>To stay connected and monitor your child's progress, please create your parent account by clicking the button below:</p>
                <br/>
                <a href='{invitationUrl}' 
                   style='background-color:#58ACAA;color:white;
                          padding:10px 20px;text-decoration:none;
                          border-radius:5px;'>Create Parent Account</a>
                <br/><br/>
                <p>This link will expire in <b>3 days</b> for security reasons.</p>
                <p>If you did not expect this invitation, please ignore this message.</p>
                <br/>
                <p>Best regards,</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendStudentLinkedAsync(Parent parent, Student student, string emailConfirmationURL)
        {
            await _emailSender.SendEmailAsync(
                parent.Email,
                "📘 You’re Now Linked to Your Student on Sconce",
                $@"
                <h2>Hi {parent.FullName},</h2>
                <p>Your Sconce account has been created and successfully linked to your student:</p>
                <ul>
                    <li><b>Student Name:</b> {student.FullName}</li>
                    <li><b>Email:</b> {student.Email}</li>
                </ul>
                <p>To activate your account, please confirm your email by clicking the button below:</p>
                <a href='{emailConfirmationURL}' 
                   style='background-color:#58ACAA;color:white;padding:10px 20px;
                          text-decoration:none;border-radius:5px;'>Confirm Email</a>
                <br/><br/>
                <p style='font-size:12px;color:#777;'>If the button doesn’t work, copy and paste this link into your browser:<br/>{emailConfirmationURL}</p>
                <br/>
                <p>Once confirmed, you can view their learning activities, monitor progress, and stay engaged in their educational journey.</p>
                <p>If you didn’t request this link, please reach out to our support team immediately.</p>
                <br/>
                <p>Welcome to the Sconce family! 🎓</p>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        // General Account Emails
        public async Task SendConfirmEmailAsync(ApplicationUser user, string emailConfirmationURL)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Welcome to Sconce – Confirm Your Email",
                $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Welcome to <span style='color: #58ACAA;'>Sconce</span>!</h2>
                    <p>Hi <strong>{user.FullName}</strong>,</p>

                    <p>We’re thrilled to have you join our learning community! To get started, please confirm your email address by clicking the button below:</p>

                    <div style='text-align: center; margin: 25px 0;'>
                        <a href='{emailConfirmationURL}' 
                            style='background-color: #58ACAA; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Confirm Email
                        </a>
                    </div>

                    <p>If the button doesn’t work, you can also copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #58ACAA;'>{emailConfirmationURL}</p>

                    <p>Thank you for joining us – we’re excited to see you shine!</p>

                    <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>
                </div>"
            );
        }

        public async Task SendPasswordResetCodeAsync(ForgotPasswordRequest forgotPasswordRequest, string code)
        {
            await _emailSender.SendEmailAsync(
                forgotPasswordRequest.Email,
                "Reset Your Sconce Password",
                $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Reset Your <span style='color: #58ACAA;'>Sconce</span> Password</h2>

                    <p>Hello,</p>
                    <p>We received a request to reset your password. Use the verification code below to proceed:</p>

                    <div style='text-align: center; margin: 25px 0;'>
                        <div style='display: inline-block; background-color: #58ACAA; color: white; padding: 12px 25px; border-radius: 5px; font-size: 24px; letter-spacing: 2px; font-weight: bold;'>
                            {code}
                        </div>
                    </div>

                    <p>This code will expire shortly, so be sure to use it soon. If you didn’t request a password reset, you can safely ignore this email.</p>

                    <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>
                </div>"
            );
        }

        public async Task SendPasswordResetSuccessAsync(ResetPasswordRequest resetPasswordRequest, ApplicationUser user)
        {
            await _emailSender.SendEmailAsync(
                resetPasswordRequest.Email,
                "Your Sconce Password Has Been Updated",
                $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Password Reset Successful</h2>

                    <p>Hi <strong>{user.FullName}</strong>,</p>

                    <p>Your password has been successfully updated. If you made this change, no further action is needed.</p>

                    <p>If you did <strong>not</strong> request this change, please visit your Sconce account and use the &quot;Forgot Password&quot; option or contact support.</p>

                    <p>Stay secure,</p>
                    <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>
                </div>"
            );
        }

        // Submission Emails
        public async Task SendSubmissionCreatedAsync(Submission submission, Assignment assignment)
        {
            if (submission.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                submission.Student.Email,
                $"Submission received for {assignment.Title}",
                $@"
                <h2>Hello {submission.Student.FullName},</h2>
                <p>We have received your submission for <b>{assignment.Title}</b>.</p>
                <p><b>Submitted at:</b> {submission.SubmittedAt:u}</p>
                <p>If you need to make changes before the deadline, you can resubmit.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendSubmissionUpdatedAsync(Submission submission, Assignment assignment)
        {
            if (submission.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                submission.Student.Email,
                $"Submission updated for {assignment.Title}",
                $@"
                <h2>Hello {submission.Student.FullName},</h2>
                <p>Your submission for <b>{assignment.Title}</b> has been updated.</p>
                <p><b>Updated at:</b> {submission.UpdatedAt:u}</p>
                <p>If this wasn’t you, please contact support immediately.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendSubmissionDeletedAsync(Submission submission, Assignment assignment)
        {
            if (submission.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                submission.Student.Email,
                $"Submission deleted for {assignment.Title}",
                $@"
                <h2>Hello {submission.Student.FullName},</h2>
                <p>Your submission for <b>{assignment.Title}</b> has been deleted.</p>
                <p>If you deleted this by mistake, you may resubmit before the deadline.</p>
                <p>If this wasn’t you, please contact support immediately.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendSubmissionGradedAsync(Submission submission, Assignment assignment)
        {
            if (submission.Student?.Email == null)
                return;

            var gradeText = submission.Grade.HasValue ? submission.Grade.Value.ToString("0.##") : "N/A";
            var feedback = string.IsNullOrWhiteSpace(submission.Feedback) ? "No feedback provided." : submission.Feedback;

            await _emailSender.SendEmailAsync(
                submission.Student.Email,
                $"Your {assignment.Title} submission has been graded",
                $@"
                <h2>Hello {submission.Student.FullName},</h2>
                <p>Your submission for <b>{assignment.Title}</b> has been graded.</p>
                <ul>
                    <li><b>Grade:</b> {gradeText}</li>
                    <li><b>Feedback:</b> {feedback}</li>
                    <li><b>Graded at:</b> {submission.GradedAt:u}</li>
                </ul>
                <p>Keep up the great work!</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        // Dropout Emails
        public async Task SendDropoutRequestedAsync(Dropout dropout)
        {
            if (dropout.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                dropout.Student.Email,
                "Dropout request submitted",
                $@"
                <h2>Hello {dropout.Student.FullName},</h2>
                <p>We have received your dropout request for <b>{dropout.Level?.Name ?? "the program"}</b>.</p>
                <p><b>Submitted at:</b> {dropout.CreatedAt:u}</p>
                <p><b>Status:</b> {dropout.ApplicationStatus}</p>
                <p>Your request will be reviewed by our team. We'll notify you once a decision has been made.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendDropoutUpdatedAsync(Dropout dropout)
        {
            if (dropout.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                dropout.Student.Email,
                "Dropout request updated",
                $@"
                <h2>Hello {dropout.Student.FullName},</h2>
                <p>Your dropout request for <b>{dropout.Level?.Name ?? "the program"}</b> has been updated.</p>
                <p><b>Updated at:</b> {dropout.UpdatedAt:u}</p>
                <p>If this wasn't you, please contact support immediately.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendDropoutCancelledAsync(Dropout dropout)
        {
            if (dropout.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                dropout.Student.Email,
                "Dropout request cancelled",
                $@"
                <h2>Hello {dropout.Student.FullName},</h2>
                <p>Your dropout request for <b>{dropout.Level?.Name ?? "the program"}</b> has been cancelled.</p>
                <p>If you cancelled this by mistake, you can submit a new request.</p>
                <p>If this wasn't you, please contact support immediately.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendDropoutApprovedAsync(Dropout dropout)
        {
            if (dropout.Student?.Email == null)
                return;

            await _emailSender.SendEmailAsync(
                dropout.Student.Email,
                "Your dropout request has been approved",
                $@"
                <h2>Hello {dropout.Student.FullName},</h2>
                <p>Your dropout request for <b>{dropout.Level?.Name ?? "the program"}</b> has been <b>approved</b>.</p>
                <p>We’re processing the necessary steps. If you have questions, please reach out to support.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }

        public async Task SendDropoutRejectedAsync(Dropout dropout, string feedback)
        {
            if (dropout.Student?.Email == null)
                return;

            var feedbackText = string.IsNullOrWhiteSpace(feedback) ? "No feedback provided." : feedback;

            await _emailSender.SendEmailAsync(
                dropout.Student.Email,
                "Your dropout request has been reviewed",
                $@"
                <h2>Hello {dropout.Student.FullName},</h2>
                <p>Your dropout request for <b>{dropout.Level?.Name ?? "the program"}</b> has been <b>rejected</b>.</p>
                <p><b>Feedback:</b> {feedbackText}</p>
                <p>If you believe this is an error or need further clarification, please contact support.</p>
                <br/>
                <p style='margin-top: 30px; color: #999;'>— The Sconce Team</p>"
            );
        }
    }
}
