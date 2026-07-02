using ElroukenAljamil.Media.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Media.Infrastructure.Persistence
{
    public class MediaDbContext : DbContext
    {
        public DbSet<MediaFile> MediaFiles => Set<MediaFile>();

        public MediaDbContext(DbContextOptions<MediaDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<MediaFile>(entity =>
            {
                entity.ToTable("MediaFiles");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OriginalFileName).IsRequired().HasMaxLength(256);
                entity.Property(e => e.StoragePath).IsRequired().HasMaxLength(500);
                entity.Property(e => e.ContentType).IsRequired().HasMaxLength(50);
                entity.Property(e => e.FileSize).IsRequired();
                entity.Property(e => e.BucketName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.OwnerId).IsRequired();
                entity.Property(e => e.SortOrder).HasDefaultValue(0);

                // Value Object : ImageDimensions
                entity.OwnsOne(e => e.Dimensions, dim =>
                {
                    dim.Property(d => d.Width).HasColumnName("Width").IsRequired();
                    dim.Property(d => d.Height).HasColumnName("Height").IsRequired();
                });

                // Value Object : MediaVariants
                entity.OwnsOne(e => e.Variants, variants =>
                {
                    variants.Property(v => v.ThumbnailPath).HasColumnName("ThumbnailPath").HasMaxLength(500);
                    variants.Property(v => v.MediumPath).HasColumnName("MediumPath").HasMaxLength(500);
                    variants.Property(v => v.LargePath).HasColumnName("LargePath").HasMaxLength(500);
                    variants.Property(v => v.WebPPath).HasColumnName("WebPPath").HasMaxLength(500);
                });

                // Enum conversion
                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                // Index
                entity.HasIndex(e => e.OwnerId);
                entity.HasIndex(e => e.ListingId);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });
        }
    }


}
