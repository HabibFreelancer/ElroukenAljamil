using System.Text.Json;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations
{
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.ToTable("Listings");
            builder.HasKey(e => e.Id);

            builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Description).IsRequired().HasMaxLength(5000);
            builder.Property(e => e.SellerName).IsRequired().HasMaxLength(100);
            builder.Property(e => e.SellerId).IsRequired();
            builder.Property(e => e.ViewCount).HasDefaultValue(0);

            // Value Object : Money
            builder.OwnsOne(e => e.Price, price =>
            {
                price.Property(p => p.Amount).HasColumnName("Price").HasPrecision(18, 2).IsRequired();
                price.Property(p => p.Currency).HasColumnName("Currency").HasMaxLength(3).IsRequired();
            });

            // Value Object : Category
            builder.OwnsOne(e => e.Category, category =>
            {
                category.Property(c => c.Name).HasColumnName("Category").HasMaxLength(100).IsRequired();
            });

            // Value Object : Location
            builder.OwnsOne(e => e.Location, location =>
            {
                location.Property(l => l.City).HasColumnName("City").HasMaxLength(100).IsRequired();
                location.Property(l => l.Latitude).HasColumnName("Latitude");
                location.Property(l => l.Longitude).HasColumnName("Longitude");
            });

            // Enum
            builder.Property(e => e.Status).HasConversion<string>().HasMaxLength(20).IsRequired();

            // ImageUrls - Note: jsonb est spécifique à PostgreSQL. 
            // Si vous êtes sur SQL Server, utilisez .HasColumnType("nvarchar(max)")
            builder.Property(e => e.ImageUrls)
                .HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null) ?? new List<string>())
                .HasColumnType("nvarchar(max)"); // Pour SQL Server

            builder.HasIndex(e => e.SellerId);
            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.CreatedAt);
            builder.HasIndex(e => e.ExpiresAt);
        }
    }

}
