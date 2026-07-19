using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface ITemplateRepository : IRepository<NotificationTemplate>
    {
        Task<NotificationTemplate?> GetActiveAsync(NotificationType type, NotificationChannel channel, string language = "fr", CancellationToken ct = default);
        Task<IReadOnlyList<NotificationTemplate>> GetAllActiveAsync(CancellationToken ct = default);
    }
}
