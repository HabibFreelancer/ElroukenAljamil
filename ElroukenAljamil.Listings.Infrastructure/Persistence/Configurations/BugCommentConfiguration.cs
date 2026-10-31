using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;

public class BugCommentConfiguration : IEntityTypeConfiguration<BugComment>
{
    public void Configure(EntityTypeBuilder<BugComment> builder)
    {
        builder.ToTable("BugComments");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.AuthorId).IsRequired().HasMaxLength(100);
        builder.Property(e => e.AuthorName).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Content).IsRequired().HasMaxLength(2000);

        builder.HasIndex(e => e.BugReportId);
        builder.HasIndex(e => e.CreatedAt);
    }
}
