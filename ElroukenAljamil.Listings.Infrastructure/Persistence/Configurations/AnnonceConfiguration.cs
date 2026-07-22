using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;

public class AnnonceConfiguration : IEntityTypeConfiguration<Annonce>
{
    public void Configure(EntityTypeBuilder<Annonce> builder)
    {
        builder.ToTable("Annonces");
        builder.HasKey(e => e.Id);

        builder.Property(e => e.Title).IsRequired().HasMaxLength(200);
        builder.Property(e => e.Description).HasMaxLength(5000);
        builder.Property(e => e.Price).HasPrecision(18, 2);
        builder.Property(e => e.AdType).HasMaxLength(100);
        builder.Property(e => e.Condition).HasMaxLength(100);
        builder.Property(e => e.Location).HasMaxLength(200);
        builder.Property(e => e.Phone).HasMaxLength(50);
        builder.Property(e => e.Email).HasMaxLength(200);
        builder.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("published");
        builder.Property(e => e.UserId).HasMaxLength(100);
        builder.Property(e => e.ExtraData).HasColumnType("nvarchar(max)").HasDefaultValue("{}");

        builder.HasOne(e => e.Category)
            .WithMany()
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(e => e.Favorites)
            .WithOne(f => f.Annonce)
            .HasForeignKey(f => f.AnnonceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(e => e.Views)
            .WithOne(v => v.Annonce)
            .HasForeignKey(v => v.AnnonceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(e => e.UserId);
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.CategoryId);
        builder.HasIndex(e => e.CreatedAt);
    }
}

public class AnnonceFavoriteConfiguration : IEntityTypeConfiguration<AnnonceFavorite>
{
    public void Configure(EntityTypeBuilder<AnnonceFavorite> builder)
    {
        builder.ToTable("AnnonceFavorites");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).IsRequired().HasMaxLength(100);
        builder.HasIndex(e => new { e.AnnonceId, e.UserId }).IsUnique();
    }
}

public class AnnonceViewConfiguration : IEntityTypeConfiguration<AnnonceView>
{
    public void Configure(EntityTypeBuilder<AnnonceView> builder)
    {
        builder.ToTable("AnnonceViews");
        builder.HasKey(e => e.Id);
        builder.Property(e => e.UserId).HasMaxLength(100);
        builder.HasIndex(e => e.AnnonceId);
    }
}
