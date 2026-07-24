using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<AnnonceCategory>
    {
        public void Configure(EntityTypeBuilder<AnnonceCategory> builder)
        {
            builder.ToTable("Categories");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Name).IsRequired().HasMaxLength(200);
            builder.Property(e => e.Slug).IsRequired().HasMaxLength(200);

            builder.HasOne(e => e.Menu)
                   .WithMany(m => m.Categories)
                   .HasForeignKey(e => e.MenuId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(e => e.ParentCategory)
                   .WithMany(c => c.SubCategories)
                   .HasForeignKey(e => e.ParentCategoryId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(e => new { e.MenuId, e.Slug });
        }
    }
}
