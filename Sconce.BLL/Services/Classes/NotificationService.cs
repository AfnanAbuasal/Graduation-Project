using Microsoft.AspNetCore.Identity.UI.Services;
using Sconce.BLL.Services.Interfaces;
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

        public async Task SendApplicationApprovedAsync(InstructorApplication app, string password)
        {
            await _emailSender.SendEmailAsync(
                app.Email,
                "🎉 Your Instructor Application Has Been Approved!",
                $@"
                <h2>Congratulations, {app.FullName}!</h2>
                <p>We’re excited to let you know that your application to join Sconce has been <b>approved</b>.</p>
                <p>You can now log in to your instructor account using these credentials:</p>
                <ul>
                    <li><b>Email:</b> {app.Email}</li>
                    <li><b>Password:</b> {password}</li>
                </ul>
                <p>Please log in and change your password as soon as possible for security.</p>
                <br/>
                <p>Welcome aboard! 🌟<br/>— The Sconce Team</p>"
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
                <p>You’re always welcome to reapply in the future. 💛<br/>— The Sconce Team</p>"
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
    }
}
