using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using FirebaseAdmin.Messaging;

namespace ElroukenAljamil.Notification.Infrastructure.Senders
{
    public class PushSender : INotificationSender
    {
        public NotificationChannel Channel => NotificationChannel.Push;

        public async Task SendAsync(NotificationRecord notification, CancellationToken ct = default)
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
        }
    }
}
