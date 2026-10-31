using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;

public class BugAttachmentConfiguration : IEntityTypeConfiguration<BugAttachment>
{
    public void Configure(EntityTypeBuilder<BugAttachment> builder)
    {
        builder.ToTable("BugAttachments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.FileName).IsRequired().HasMaxLength(300);
        builder.Property(e => e.StorageUrl).IsRequired().HasMaxLength(1000);
        builder.Property(e => e.ContentType).IsRequired().HasMaxLength(100);
        builder.Property(e => e.FileSize).IsRequired();
        builder.Property(e => e.UploadedByUserId).IsRequired().HasMaxLength(100);

        builder.HasIndex(e => e.BugReportId);
    }
}
