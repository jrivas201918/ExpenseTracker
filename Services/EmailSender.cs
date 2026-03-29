using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace ExpenseTracker.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly IConfiguration _config;

        public EmailSender(ILogger<EmailSender> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            _logger.LogInformation($"Sending email to {email} with subject {subject}");

            var smtpServer = _config["SmtpSettings:Server"] ?? "smtp.gmail.com";
            var smtpPort = int.TryParse(_config["SmtpSettings:Port"], out var port) ? port : 587;
            var smtpUser = _config["SmtpSettings:Username"] ?? "expensetracker201918@gmail.com";
            var smtpPass = _config["SmtpSettings:Password"] ?? "dummy-password";

            try
            {
                var client = new SmtpClient(smtpServer, smtpPort)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(smtpUser, smtpPass)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(smtpUser, "Expense Tracker Security"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                // return client.SendMailAsync(mailMessage); 
                // We mock it for the demo if dummy password is used to avoid app crash
                if (smtpPass == "dummy-password")
                {
                    _logger.LogWarning("Dummy SMTP password detected. Email send simulated.");
                    return Task.CompletedTask;
                }

                return client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email.");
                return Task.CompletedTask; // Prevent app crashing on failed email
            }
        }
    }
}
