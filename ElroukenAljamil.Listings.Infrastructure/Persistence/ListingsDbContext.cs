using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;


namespace ElroukenAljamil.Listings.Infrastructure.Persistence
{
    public class ListingsDbContext : DbContext
    {
        public DbSet<Listing> Listings => Set<Listing>();

        public ListingsDbContext(DbContextOptions<ListingsDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Applique cette configuration spécifique
            modelBuilder.ApplyConfiguration(new ListingConfiguration());
        }

        /// <summary>
        /// Dispatch les domain events avant de sauvegarder.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Récupérer les domain events avant sauvegarde
            var domainEvents = ChangeTracker.Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            // Sauvegarder en base
            var result = await base.SaveChangesAsync(cancellationToken);

            // Nettoyer les events après persistance
            foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
            {
                entry.Entity.ClearDomainEvents();
            }

            return result;
        }
    }


}
