using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Text.Json;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class SmsSender : INotificationSender
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;
        private readonly ILogger<SmsSender> _logger;

        public SmsSender(HttpClient http, IConfiguration config, ILogger<SmsSender> logger)
        {
            _http = http;
            _config = config;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.Sms;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
        {
            _logger.LogInformation("Envoi SMS notification {NotificationId} à {Recipient}",
                notification.Id, notification.Metadata);
            try
            {
                var payload = new
                {
                    sender = _config["Brevo:SmsSenderName"] ?? "Marketplace",
                    recipient = notification.Metadata ?? "",
                    content = notification.Body
                };

                var request = new HttpRequestMessage(HttpMethod.Post, "https://api.brevo.com/v3/transactionalSMS/sms");
                request.Headers.Add("api-key", _config["Brevo:ApiKey"]);
                request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _http.SendAsync(request, ct);
                if (!response.IsSuccessStatusCode)
                    _logger.LogWarning("Échec envoi SMS notification {NotificationId} — HTTP {StatusCode}",
                        notification.Id, (int)response.StatusCode);
                else
                    _logger.LogInformation("SMS notification {NotificationId} envoyé avec succès", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec envoi SMS notification {NotificationId} à {Recipient}",
                    notification.Id, notification.Metadata);
                throw;
            }
        }
    }
}
