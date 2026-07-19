using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public class PreferenceRepository : BaseRepository<UserNotificationPreference>, IPreferenceRepository
    {
        public PreferenceRepository(NotificationDbContext context) : base(context) { }

        public async Task<UserNotificationPreference?> GetByUserAndTypeAsync(Guid userId, NotificationType type, CancellationToken ct = default)
            => await DbSet.FirstOrDefaultAsync(p => p.UserId == userId && p.NotificationType == type, ct);

        public async Task<IReadOnlyList<UserNotificationPreference>> GetByUserAsync(Guid userId, CancellationToken ct = default)
            => await DbSet.Where(p => p.UserId == userId).ToListAsync(ct);
    }
}
