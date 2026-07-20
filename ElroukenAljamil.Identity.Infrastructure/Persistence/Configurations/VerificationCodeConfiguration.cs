using ElroukenAljamil.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElroukenAljamil.Identity.Infrastructure.Persistence.Configurations
{
    public class VerificationCodeConfiguration : IEntityTypeConfiguration<VerificationCode>
    {
        public void Configure(EntityTypeBuilder<VerificationCode> builder)
        {
            builder.ToTable("VerificationCodes");
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Target).IsRequired().HasMaxLength(300);
            builder.Property(e => e.Code).IsRequired().HasMaxLength(10);
            builder.HasIndex(e => e.Target);
        }
    }
}
