using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace Sconce.PL;

public class EmailSetting : IEmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSetting(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _configuration["MailSettings:Host"];
        var port = int.Parse(_configuration["MailSettings:Port"]);
        var enableSsl = bool.Parse(_configuration["MailSettings:EnableSSL"]);
        var fromEmail = _configuration["MailSettings:Email"];
        var password = _configuration["MailSettings:Password"];
        var displayName = _configuration["MailSettings:DisplayName"];

        var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(fromEmail, password)
        };

        var message = new MailMessage(from: new MailAddress(fromEmail, displayName), to: new MailAddress(email))
        {
            Subject = subject,
            Body = htmlMessage,
            IsBodyHtml = true
        };

        return client.SendMailAsync(message);
    }
}
