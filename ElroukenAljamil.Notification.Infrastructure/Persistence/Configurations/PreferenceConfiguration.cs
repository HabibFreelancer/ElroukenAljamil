using ElroukenAljamil.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Configurations
{
    public class PreferenceConfiguration : IEntityTypeConfiguration<UserNotificationPreference>
    {
        public void Configure(EntityTypeBuilder<UserNotificationPreference> builder)
        {
            builder.HasKey(p => p.Id);
            builder.Property(p => p.NotificationType).HasConversion<string>();
            builder.HasIndex(p => new { p.UserId, p.NotificationType }).IsUnique();
        }
    }
}
