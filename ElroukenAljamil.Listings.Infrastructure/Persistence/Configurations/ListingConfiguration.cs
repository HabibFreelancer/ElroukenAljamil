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


            builder.HasKey(l => l.Id);


            builder.Property(l => l.Title)
                .HasMaxLength(150)
                .IsRequired();


            builder.Property(l => l.Description)
                .HasMaxLength(5000)
                .IsRequired();


            builder.Property(l => l.Status)
                .HasConversion<string>()
                .HasMaxLength(20);


            // Mapping du Value Object Price (Owned Entity)
            builder.OwnsOne(l => l.Price, price =>
            {
                price.Property(p => p.Amount)
                    .HasColumnName("Price")
                    .HasColumnType("decimal(18,2)")
                    .IsRequired();


                price.Property(p => p.Currency)
                    .HasColumnName("Currency")
                    .HasMaxLength(3)
                    .IsRequired();
            });


            // Mapping du Value Object Address (Owned Entity)
            builder.OwnsOne(l => l.Location, address =>
            {
                address.Property(a => a.City)
                    .HasColumnName("City")
                    .HasMaxLength(100)
                    .IsRequired();


                address.Property(a => a.PostalCode)
                    .HasColumnName("PostalCode")
                    .HasMaxLength(10)
                    .IsRequired();


                address.Property(a => a.Country)
                    .HasColumnName("Country")
                    .HasMaxLength(2)
                    .IsRequired();


                address.Property(a => a.Latitude).HasColumnName("Latitude");
                address.Property(a => a.Longitude).HasColumnName("Longitude");
            });


            // Relation 1-N avec les images
            builder.HasMany(l => l.Images)
                .WithOne()
                .HasForeignKey(i => i.ListingId)
                .OnDelete(DeleteBehavior.Cascade);


            // Index pour les requêtes fréquentes
            builder.HasIndex(l => l.SellerId);
            builder.HasIndex(l => l.CategoryId);
            builder.HasIndex(l => l.Status);
            builder.HasIndex(l => l.CreatedAt);


            // Ignorer les domain events pour la persistance
            builder.Ignore(l => l.DomainEvents);
        }
    }

}
