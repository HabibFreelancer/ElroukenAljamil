using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace ElroukenAljamil.Listings.Infrastructure.Persistence
{
    public class ListingsDbContext : DbContext
    {
        public DbSet<ListingMenu> Menus => Set<ListingMenu>();
        public DbSet<ListingCategory> Categories => Set<ListingCategory>();
        public DbSet<ListingAdType> AdTypes => Set<ListingAdType>();
        public DbSet<DepositWorkflow> DepositWorkflows => Set<DepositWorkflow>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<StepField> StepFields => Set<StepField>();
        public DbSet<Annonce> Annonces => Set<Annonce>();
        public DbSet<AnnonceFavorite> AnnonceFavorites => Set<AnnonceFavorite>();
        public DbSet<AnnonceView> AnnonceViews => Set<AnnonceView>();

        public ListingsDbContext(DbContextOptions<ListingsDbContext> options)
            : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new MenuConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new AdTypeConfiguration());
            modelBuilder.ApplyConfiguration(new DepositWorkflowConfiguration());
            modelBuilder.ApplyConfiguration(new WorkflowStepConfiguration());
            modelBuilder.ApplyConfiguration(new StepFieldConfiguration());
            modelBuilder.ApplyConfiguration(new AnnonceConfiguration());
            modelBuilder.ApplyConfiguration(new AnnonceFavoriteConfiguration());
            modelBuilder.ApplyConfiguration(new AnnonceViewConfiguration());
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
