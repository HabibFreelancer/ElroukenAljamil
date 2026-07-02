using ElroukenAljamil.Messaging.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Messaging.Infrastructure.Persistence
{
    public class MessagingDbContext : DbContext
    {
        public DbSet<Conversation> Conversations => Set<Conversation>();
        public DbSet<Message> Messages => Set<Message>();

        public MessagingDbContext(DbContextOptions<MessagingDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ToTable("Conversations");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.BuyerId).IsRequired();
                entity.Property(e => e.BuyerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.SellerId).IsRequired();
                entity.Property(e => e.SellerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ListingId).IsRequired();
                entity.Property(e => e.ListingTitle).IsRequired().HasMaxLength(200);
                entity.Property(e => e.UnreadCountBuyer).HasDefaultValue(0);
                entity.Property(e => e.UnreadCountSeller).HasDefaultValue(0);

                entity.Property(e => e.Status)
                    .HasConversion<string>()
                    .HasMaxLength(20)
                    .IsRequired();

                // Relation avec les messages
                entity.HasMany(e => e.Messages)
                    .WithOne()
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Index pour les requêtes fréquentes
                entity.HasIndex(e => e.BuyerId);
                entity.HasIndex(e => e.SellerId);
                entity.HasIndex(e => e.ListingId);
                entity.HasIndex(e => e.LastMessageAt);
                entity.HasIndex(e => new { e.BuyerId, e.SellerId, e.ListingId }).IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ToTable("Messages");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ConversationId).IsRequired();
                entity.Property(e => e.SenderId).IsRequired();
                entity.Property(e => e.SenderName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Content).IsRequired().HasMaxLength(2000);
                entity.Property(e => e.SentAt).IsRequired();
                entity.Property(e => e.IsRead).HasDefaultValue(false);
                entity.Property(e => e.IsEdited).HasDefaultValue(false);
                entity.Property(e => e.IsDeleted).HasDefaultValue(false);

                // Index
                entity.HasIndex(e => e.ConversationId);
                entity.HasIndex(e => e.SenderId);
                entity.HasIndex(e => e.SentAt);
            });
        }
    }
}
