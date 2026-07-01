using ElroukenAljamil.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Identity.Infrastructure.Persistence
{
    public class IdentityDbContext : DbContext
    {
        public DbSet<ApplicationUser> Users => Set<ApplicationUser>();

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.UserName).IsRequired().HasMaxLength(50);
                entity.Property(e => e.PasswordHash).IsRequired();
                entity.Property(e => e.FirstName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.LastName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.PhoneNumber).HasMaxLength(20);
                entity.Property(e => e.AvatarUrl).HasMaxLength(500);
                entity.Property(e => e.RefreshToken).HasMaxLength(256);

                // Index unique sur l'email et le username
                entity.HasIndex(e => e.Email).IsUnique();
                entity.HasIndex(e => e.UserName).IsUnique();
                entity.HasIndex(e => e.RefreshToken);

                // Value Object : Phone
                entity.OwnsOne(e => e.Phone, phone =>
                {
                    phone.Property(p => p.Value).HasColumnName("PhoneNumberFormatted").HasMaxLength(20);
                });

                // Value Object : Address
                entity.OwnsOne(e => e.Address, address =>
                {
                    address.Property(a => a.Street).HasColumnName("Address_Street").HasMaxLength(200);
                    address.Property(a => a.City).HasColumnName("Address_City").HasMaxLength(100);
                    address.Property(a => a.ZipCode).HasColumnName("Address_ZipCode").HasMaxLength(10);
                    address.Property(a => a.Country).HasColumnName("Address_Country").HasMaxLength(100);
                    address.Property(a => a.State).HasColumnName("Address_State").HasMaxLength(100);
                });

                // Enum conversion
                entity.Property(e => e.Role).HasConversion<string>().HasMaxLength(20);
                entity.Property(e => e.Status).HasConversion<string>().HasMaxLength(30);
            });
        }
    }
}
