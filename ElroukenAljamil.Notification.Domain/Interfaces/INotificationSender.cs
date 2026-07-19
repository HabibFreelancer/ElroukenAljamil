using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface INotificationSender
    {
        NotificationChannel Channel { get; }
        Task SendAsync(NotificationRecord notification, CancellationToken ct = default);
    }
}
