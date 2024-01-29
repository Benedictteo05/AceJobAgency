using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;

namespace AceJobAgency.Services
{
    public class EmailService : IEmailSender
    {
        private IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public Task SendEmailAsync(string email, string subject, string msg)
        {
            var sender = _configuration["EmailAddress"];
            var password = _configuration["EmailPW"];

            SmtpClient client = new("smtp-mail.outlook.com", 587)
            {
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(sender, password)
            };

            return client.SendMailAsync(new MailMessage(sender, email, subject, msg));
        }
    }
}
