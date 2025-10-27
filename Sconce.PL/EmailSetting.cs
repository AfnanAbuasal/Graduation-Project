using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Sconce.PL
{
    public class EmailSetting : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSetting(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("afnanalaa49@gmail.com", _configuration["EmailSender:AppPassword"])
            };

            return client.SendMailAsync(
                new MailMessage(from: "afnanalaa49@gmail.com",
                                to: email,
                                subject,
                                htmlMessage
                )
                { IsBodyHtml = true}
            );
        }
    }
}
