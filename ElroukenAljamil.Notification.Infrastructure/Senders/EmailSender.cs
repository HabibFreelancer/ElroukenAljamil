using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class EmailSender : INotificationSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Email;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
        {
            _logger.LogInformation("Envoi email notification {NotificationId} à {Recipient}",
                notification.Id, notification.Metadata);
            try
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
                _logger.LogInformation("Email notification {NotificationId} envoyé avec succès", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec envoi email notification {NotificationId} à {Recipient}",
                    notification.Id, notification.Metadata);
                throw;
            }
        }
    }
}
