using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;

namespace RF1.Services.EmailService
{
    public class EmailService
    {
        private readonly SmtpSettings _smtpSettings;

        public EmailService(IOptions<SmtpSettings> smtpSettings)
        {
            _smtpSettings = smtpSettings.Value;
        }

        public void SendEmail(string to, string subject, string body)
        {
            var smtpPassword = Environment.GetEnvironmentVariable("MAILTRAP_PASSWORD");

            if (string.IsNullOrEmpty(smtpPassword))
            {
                throw new Exception("SMTP password not configured. Please set the SMTP_PASSWORD environment variable.");
            }

            var client = new SmtpClient(_smtpSettings.Host, _smtpSettings.Port)
            {
                Credentials = new NetworkCredential(_smtpSettings.User, smtpPassword),
                EnableSsl = _smtpSettings.EnableSsl
            };

            var fromAddress = new MailAddress(_smtpSettings.From, "realfoodapp@demomailtrap.com");
            var toAddress = new MailAddress(to);

            var mailMessage = new MailMessage(fromAddress, toAddress)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };

            client.Send(mailMessage);
        }
    }
}
