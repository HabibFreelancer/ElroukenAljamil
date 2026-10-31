using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;

public class BugReportConfiguration : IEntityTypeConfiguration<BugReport>
{
    public void Configure(EntityTypeBuilder<BugReport> builder)
    {
        builder.ToTable("BugReports");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(300);
        builder.Property(e => e.Description).IsRequired().HasMaxLength(4000);
        builder.Property(e => e.Severity).HasConversion<int>().IsRequired();
        builder.Property(e => e.Status).HasConversion<int>().IsRequired();
        builder.Property(e => e.Category).IsRequired().HasMaxLength(100);
        builder.Property(e => e.SubCategory).HasMaxLength(100);
        builder.Property(e => e.Module).HasMaxLength(100);
        builder.Property(e => e.Environment).HasMaxLength(100);
        builder.Property(e => e.StepsToReproduce).HasMaxLength(2000);
        builder.Property(e => e.ExpectedBehavior).HasMaxLength(1000);
        builder.Property(e => e.ActualBehavior).HasMaxLength(1000);
        builder.Property(e => e.ScreenshotUrl).HasMaxLength(500);
        builder.Property(e => e.BrowserInfo).HasMaxLength(200);
        builder.Property(e => e.ReportedByUserId).IsRequired().HasMaxLength(100);
        builder.Property(e => e.ReportedByUserName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.AssignedToUserId).HasMaxLength(100);

        builder.HasMany(e => e.Comments)
            .WithOne(c => c.BugReport)
            .HasForeignKey(c => c.BugReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Attachments)
            .WithOne(a => a.BugReport)
            .HasForeignKey(a => a.BugReportId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.Severity);
        builder.HasIndex(e => e.ReportedByUserId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
