using IMHub.ApplicationLayer.Common.Interfaces.Infrastruture;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace IMHub.Infrastructure.Services
{
    public class SendGridEmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public SendGridEmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            // AppSettings-ல் இருந்து Key-ஐ எடுப்போம்
            var apiKey = _configuration["SendGrid:ApiKey"];
            var fromEmail = _configuration["SendGrid:FromEmail"];
            var fromName = _configuration["SendGrid:FromName"];

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(toEmail);
            var plainTextContent = message;
            var htmlContent = message; // HTML Email அனுப்ப வேண்டுமென்றால் இதை மாற்றலாம்

            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            var response = await client.SendEmailAsync(msg);

            // Future: நாம் உருவாக்கிய 'NotificationLog' டேபிளில் இதை லாக் செய்யலாம்.
        }
    }
}