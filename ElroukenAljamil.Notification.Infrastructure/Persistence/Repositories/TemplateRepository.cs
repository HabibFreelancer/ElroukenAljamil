using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public class TemplateRepository : BaseRepository<NotificationTemplate>, ITemplateRepository
    {
        public TemplateRepository(NotificationDbContext context) : base(context) { }

        public async Task<NotificationTemplate?> GetActiveAsync(NotificationType type, NotificationChannel channel, string language = "fr", CancellationToken ct = default)
            => await DbSet.FirstOrDefaultAsync(t => t.Type == type && t.Channel == channel && t.Language == language && t.IsActive, ct);

        public async Task<IReadOnlyList<NotificationTemplate>> GetAllActiveAsync(CancellationToken ct = default)
            => await DbSet.Where(t => t.IsActive).ToListAsync(ct);
    }
}
