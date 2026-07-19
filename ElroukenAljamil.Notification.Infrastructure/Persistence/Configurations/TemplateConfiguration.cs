using ElroukenAljamil.Notification.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Configurations
{
    public class TemplateConfiguration : IEntityTypeConfiguration<NotificationTemplate>
    {
        public void Configure(EntityTypeBuilder<NotificationTemplate> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TitleTemplate).HasMaxLength(500).IsRequired();
            builder.Property(t => t.BodyTemplate).HasMaxLength(8000).IsRequired();
            builder.Property(t => t.Language).HasMaxLength(10).IsRequired();
            builder.Property(t => t.Type).HasConversion<string>();
            builder.Property(t => t.Channel).HasConversion<string>();
            builder.HasIndex(t => new { t.Type, t.Channel, t.Language, t.IsActive });
        }
    }
}
