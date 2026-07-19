using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public class NotificationRepository : BaseRepository<NotificationRecord>, INotificationRepository
    {
        public NotificationRepository(NotificationDbContext context) : base(context) { }

        public async Task<IReadOnlyList<NotificationRecord>> GetByRecipientAsync(Guid recipientId, int page, int pageSize, CancellationToken ct = default)
            => await DbSet.Where(n => n.RecipientId == recipientId).OrderByDescending(n => n.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);

        public async Task<IReadOnlyList<NotificationRecord>> GetPendingAsync(int batchSize, CancellationToken ct = default)
            => await DbSet.Where(n => n.Status == NotificationStatus.Pending && n.RetryCount < 3).OrderBy(n => n.CreatedAt).Take(batchSize).ToListAsync(ct);

        public async Task<IReadOnlyList<NotificationRecord>> GetUnreadInAppAsync(Guid recipientId, CancellationToken ct = default)
            => await DbSet.Where(n => n.RecipientId == recipientId && n.Channel == NotificationChannel.InApp && n.ReadAt == null).OrderByDescending(n => n.CreatedAt).ToListAsync(ct);

        public async Task<IReadOnlyList<NotificationRecord>> GetUnreadByRecipientAsync(Guid recipientId, CancellationToken ct = default)
            => await DbSet.Where(n => n.RecipientId == recipientId && n.ReadAt == null).OrderByDescending(n => n.CreatedAt).ToListAsync(ct);

        public async Task<int> CountUnreadAsync(Guid recipientId, CancellationToken ct = default)
            => await DbSet.CountAsync(n => n.RecipientId == recipientId && n.Channel == NotificationChannel.InApp && n.ReadAt == null, ct);
    }
}
