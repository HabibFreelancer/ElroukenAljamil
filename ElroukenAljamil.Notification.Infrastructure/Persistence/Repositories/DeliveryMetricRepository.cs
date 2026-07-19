using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public class DeliveryMetricRepository : BaseRepository<DeliveryMetric>, IDeliveryMetricRepository
    {
        public DeliveryMetricRepository(NotificationDbContext context) : base(context) { }

        public async Task<DeliveryMetric?> GetOrCreateForPeriodAsync(NotificationChannel channel, NotificationType type, DateTime periodStart, CancellationToken ct = default)
        {
            var hour = new DateTime(periodStart.Year, periodStart.Month, periodStart.Day, periodStart.Hour, 0, 0, DateTimeKind.Utc);
            var metric = await DbSet.FirstOrDefaultAsync(m => m.Channel == channel && m.Type == type && m.PeriodStart == hour, ct);
            if (metric is not null) return metric;

            metric = DeliveryMetric.Create(channel, type, hour);
            await DbSet.AddAsync(metric, ct);
            await Context.SaveChangesAsync(ct);
            return metric;
        }

        public async Task<IReadOnlyList<DeliveryMetric>> GetByPeriodAsync(DateTime from, DateTime to, CancellationToken ct = default)
            => await DbSet.Where(m => m.PeriodStart >= from && m.PeriodStart <= to).ToListAsync(ct);

        public async Task<IReadOnlyList<DeliveryMetric>> GetByChannelAndPeriodAsync(NotificationChannel channel, DateTime from, DateTime to, CancellationToken ct = default)
            => await DbSet.Where(m => m.Channel == channel && m.PeriodStart >= from && m.PeriodStart <= to).ToListAsync(ct);
    }
}
