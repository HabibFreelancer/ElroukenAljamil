using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class InAppSender : INotificationSender
    {
        private readonly IRealTimeNotificationService _realTime;
        private readonly ILogger<InAppSender> _logger;

        public InAppSender(IRealTimeNotificationService realTime, ILogger<InAppSender> logger)
        {
            _realTime = realTime;
            _logger = logger;
        }

        public NotificationChannel Channel => NotificationChannel.InApp;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
        {
            _logger.LogDebug("Envoi notification in-app {NotificationId} à {RecipientId}",
                notification.Id, notification.RecipientId);
            try
            {
                await _realTime.SendToUserAsync(notification.RecipientId, new RealTimeNotificationPayload
                {
                    Id = notification.Id,
                    Type = notification.Type.ToString(),
                    Title = notification.Title,
                    Body = notification.Body,
                    Metadata = notification.Metadata,
                    CreatedAt = notification.CreatedAt
                }, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec envoi notification in-app {NotificationId} à {RecipientId}",
                    notification.Id, notification.RecipientId);
                throw;
            }
        }
    }
}
