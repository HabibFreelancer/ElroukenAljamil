using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;

public class FeedbackConfiguration : IEntityTypeConfiguration<Feedback>
{
    public void Configure(EntityTypeBuilder<Feedback> builder)
    {
        builder.ToTable("Feedbacks");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.UserId).HasMaxLength(100);
        builder.Property(e => e.UserEmail).HasMaxLength(200);
        builder.Property(e => e.Rating).IsRequired().HasMaxLength(50);
        builder.Property(e => e.Category).HasMaxLength(100);

        builder.HasOne(e => e.Annonce)
            .WithMany()
            .HasForeignKey(e => e.AnnonceId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasIndex(e => e.Rating);
        builder.HasIndex(e => e.CreatedAt);
    }
}
