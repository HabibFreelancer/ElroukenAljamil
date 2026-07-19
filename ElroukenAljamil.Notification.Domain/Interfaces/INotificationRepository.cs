using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface INotificationRepository : IRepository<NotificationRecord>
    {
        Task<IReadOnlyList<NotificationRecord>> GetByRecipientAsync(Guid recipientId, int page, int pageSize, CancellationToken ct = default);
        Task<IReadOnlyList<NotificationRecord>> GetPendingAsync(int batchSize, CancellationToken ct = default);
        Task<IReadOnlyList<NotificationRecord>> GetUnreadInAppAsync(Guid recipientId, CancellationToken ct = default);
        Task<IReadOnlyList<NotificationRecord>> GetUnreadByRecipientAsync(Guid recipientId, CancellationToken ct = default);
        Task<int> CountUnreadAsync(Guid recipientId, CancellationToken ct = default);
    }
}
