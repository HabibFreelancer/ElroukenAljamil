using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations
{
    public class MenuConfiguration : IEntityTypeConfiguration<AnnonceMenu>
    {
        public void Configure(EntityTypeBuilder<AnnonceMenu> builder)
        {
            builder.ToTable("Menus");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Slug).IsRequired().HasMaxLength(100);
            builder.Property(e => e.Icon).HasMaxLength(100);
            builder.HasIndex(e => e.Slug).IsUnique();
        }
    }
}
