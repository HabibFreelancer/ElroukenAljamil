using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;
using System.Text.Json;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class SmsSender : INotificationSender
    {
        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public SmsSender(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public NotificationChannel Channel => NotificationChannel.Sms;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
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

            await _http.SendAsync(request, ct);
        }
    }
}
