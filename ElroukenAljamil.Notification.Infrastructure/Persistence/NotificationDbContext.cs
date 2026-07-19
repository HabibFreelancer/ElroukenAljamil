using ElroukenAljamil.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence
{
    public class NotificationDbContext : DbContext
    {
        public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

        public DbSet<NotificationRecord> Notifications => Set<NotificationRecord>();
        public DbSet<NotificationTemplate> Templates => Set<NotificationTemplate>();
        public DbSet<UserNotificationPreference> Preferences => Set<UserNotificationPreference>();
        public DbSet<DeliveryMetric> DeliveryMetrics => Set<DeliveryMetric>();
        public DbSet<DigestSchedule> DigestSchedules => Set<DigestSchedule>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(NotificationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
