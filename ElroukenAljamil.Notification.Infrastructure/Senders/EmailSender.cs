using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class EmailSender : INotificationSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config) => _config = config;

        public NotificationChannel Channel => NotificationChannel.Email;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(
                _config["Brevo:FromName"] ?? "Marketplace",
                _config["Brevo:FromEmail"] ?? "noreply@marketplace.com"));
            message.To.Add(new MailboxAddress("", notification.Metadata ?? ""));
            message.Subject = notification.Title;
            message.Body = new TextPart("html") { Text = notification.Body };

            using var client = new SmtpClient();
            await client.ConnectAsync(_config["Brevo:SmtpHost"], int.Parse(_config["Brevo:SmtpPort"] ?? "587"), false, ct);
            await client.AuthenticateAsync(_config["Brevo:SmtpUser"], _config["Brevo:SmtpPassword"], ct);
            await client.SendAsync(message, ct);
            await client.DisconnectAsync(true, ct);
        }
    }
}
