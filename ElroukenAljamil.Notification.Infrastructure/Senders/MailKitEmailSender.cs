using ElroukenAljamil.Notification.Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class MailKitEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public MailKitEmailSender(IConfiguration config) => _config = config;

        public async Task SendAsync(string toEmail, string toName, string subject, string htmlBody, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(_config["Brevo:FromName"] ?? "Marketplace", _config["Brevo:FromEmail"] ?? "noreply@marketplace.com"));
            message.To.Add(new MailboxAddress(toName, toEmail));
            message.Subject = subject;
            message.Body = new TextPart("html") { Text = htmlBody };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Brevo:SmtpHost"], int.Parse(_config["Brevo:SmtpPort"] ?? "587"), false, ct);
            await client.AuthenticateAsync(_config["Brevo:SmtpUser"], _config["Brevo:SmtpPassword"], ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
