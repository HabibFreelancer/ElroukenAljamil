using ElroukenAljamil.Common.Interfaces;
using ElroukenAljamil.Listings.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace ElroukenAljamil.Listings.Infrastructure.Persistence
{
    /// <summary>
    /// DbContext dédié au microservice Listings.
    /// Chaque microservice possède sa propre base de données (Database per Service).
    /// </summary>
    public class ListingsDbContext : DbContext, IUnitOfWork
    {
        public DbSet<Listing> Listings => Set<Listing>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<ListingImage> ListingImages => Set<ListingImage>();


        public ListingsDbContext(DbContextOptions<ListingsDbContext> options) : base(options) { }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ListingsDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

        public Task BeginTransactionAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task CommitTransactionAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync(CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }

}
