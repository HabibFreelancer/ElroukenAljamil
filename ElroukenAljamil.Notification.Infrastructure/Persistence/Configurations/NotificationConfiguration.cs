using ElroukenAljamil.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<NotificationRecord>
    {
        public void Configure(EntityTypeBuilder<NotificationRecord> builder)
        {
            builder.HasKey(n => n.Id);
            builder.Property(n => n.Title).HasMaxLength(200).IsRequired();
            builder.Property(n => n.Body).HasMaxLength(4000).IsRequired();
            builder.Property(n => n.ErrorMessage).HasMaxLength(1000);
            builder.Property(n => n.Metadata).HasMaxLength(2000);
            builder.Property(n => n.Type).HasConversion<string>();
            builder.Property(n => n.Channel).HasConversion<string>();
            builder.Property(n => n.Status).HasConversion<string>();
            builder.HasIndex(n => new { n.RecipientId, n.Status });
            builder.HasIndex(n => new { n.Status, n.RetryCount });
        }
    }
}
