using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public class DigestScheduleRepository : BaseRepository<DigestSchedule>, IDigestScheduleRepository
    {
        public DigestScheduleRepository(NotificationDbContext context) : base(context) { }

        public async Task<DigestSchedule?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
            => await DbSet.FirstOrDefaultAsync(d => d.UserId == userId, ct);

        public async Task<IReadOnlyList<DigestSchedule>> GetActiveSchedulesAsync(CancellationToken ct = default)
            => await DbSet.Where(d => d.IsActive).ToListAsync(ct);
    }
}
