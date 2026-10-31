using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using FirebaseAdmin.Messaging;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class PushSender : INotificationSender
    {
        private readonly ILogger<PushSender> _logger;

        public PushSender(ILogger<PushSender> logger) => _logger = logger;

        public NotificationChannel Channel => NotificationChannel.Push;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
        {
            _logger.LogDebug("Envoi push notification {NotificationId} à {RecipientId}",
                notification.Id, notification.RecipientId);
            try
            {
                var message = new Message
                {
                    Topic = $"user-{notification.RecipientId}",
                    Notification = new FirebaseAdmin.Messaging.Notification
                    {
                        Title = notification.Title,
                        Body = notification.Body
                    }
                };

                await FirebaseMessaging.DefaultInstance.SendAsync(message, ct);
                _logger.LogDebug("Push notification {NotificationId} envoyée avec succès", notification.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec envoi push notification {NotificationId} à {RecipientId}",
                    notification.Id, notification.RecipientId);
                throw;
            }
        }
    }
}
