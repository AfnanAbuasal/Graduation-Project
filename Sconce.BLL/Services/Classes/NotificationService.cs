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
                   style='background-color:#1abc9c;color:white;padding:10px 20px;
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
                <p>— The Sconce Team</p>"
            );
        }

        public async Task SendConfirmEmailAsync(ApplicationUser user, string emailConfirmationURL)
        {
            await _emailSender.SendEmailAsync(
                user.Email,
                "Welcome to Sconce – Confirm Your Email",
                $@"
                <div style='font-family: Arial, sans-serif; color: #333; max-width: 600px; margin: auto; padding: 20px; border: 1px solid #eee; border-radius: 10px;'>
                    <h2 style='color: #2c3e50;'>Welcome to <span style='color: #1abc9c;'>Sconce</span>!</h2>
                    <p>Hi <strong>{user.FullName}</strong>,</p>

                    <p>We’re thrilled to have you join our learning community! To get started, please confirm your email address by clicking the button below:</p>

                    <div style='text-align: center; margin: 25px 0;'>
                        <a href='{emailConfirmationURL}' 
                            style='background-color: #1abc9c; color: white; padding: 12px 25px; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                            Confirm Email
                        </a>
                    </div>

                    <p>If the button doesn’t work, you can also copy and paste this link into your browser:</p>
                    <p style='word-break: break-all; color: #1abc9c;'>{emailConfirmationURL}</p>

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
                    <h2 style='color: #2c3e50;'>Reset Your <span style='color: #1abc9c;'>Sconce</span> Password</h2>

                    <p>Hello,</p>
                    <p>We received a request to reset your password. Use the verification code below to proceed:</p>

                    <div style='text-align: center; margin: 25px 0;'>
                        <div style='display: inline-block; background-color: #1abc9c; color: white; padding: 12px 25px; border-radius: 5px; font-size: 24px; letter-spacing: 2px; font-weight: bold;'>
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

    }
}
