using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class InAppSender : INotificationSender
    {
        private readonly IRealTimeNotificationService _realTime;

        public InAppSender(IRealTimeNotificationService realTime) => _realTime = realTime;

        public NotificationChannel Channel => NotificationChannel.InApp;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
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
    }
}
