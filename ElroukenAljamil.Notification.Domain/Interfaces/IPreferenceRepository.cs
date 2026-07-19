using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface IPreferenceRepository : IRepository<UserNotificationPreference>
    {
        Task<UserNotificationPreference?> GetByUserAndTypeAsync(Guid userId, NotificationType type, CancellationToken ct = default);
        Task<IReadOnlyList<UserNotificationPreference>> GetByUserAsync(Guid userId, CancellationToken ct = default);
    }
}
