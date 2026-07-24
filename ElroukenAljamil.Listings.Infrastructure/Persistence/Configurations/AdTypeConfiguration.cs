using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations
{
    public class AdTypeConfiguration : IEntityTypeConfiguration<AnnonceAdType>
    {
        public void Configure(EntityTypeBuilder<AnnonceAdType> builder)
        {
            builder.ToTable("AdTypes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Label).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Description).HasMaxLength(500);

            builder.HasOne(e => e.Category)
                   .WithMany()
                   .HasForeignKey(e => e.CategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => e.CategoryId);
            builder.HasIndex(e => e.DisplayOrder);
        }
    }
}
