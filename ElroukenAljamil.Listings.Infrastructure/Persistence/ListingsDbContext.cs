using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Infrastructure.Persistence.Configurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;


namespace ElroukenAljamil.Listings.Infrastructure.Persistence
{
    public class ListingsDbContext : DbContext
    {
        public DbSet<AnnonceMenu> Menus => Set<AnnonceMenu>();
        public DbSet<AnnonceCategory> Categories => Set<AnnonceCategory>();
        public DbSet<AnnonceAdType> AdTypes => Set<AnnonceAdType>();
        public DbSet<DepositWorkflow> DepositWorkflows => Set<DepositWorkflow>();
        public DbSet<WorkflowStep> WorkflowSteps => Set<WorkflowStep>();
        public DbSet<StepField> StepFields => Set<StepField>();
        public DbSet<Annonce> Annonces => Set<Annonce>();
        public DbSet<AnnonceFavorite> AnnonceFavorites => Set<AnnonceFavorite>();
        public DbSet<AnnonceView> AnnonceViews => Set<AnnonceView>();
        public DbSet<Feedback> Feedbacks => Set<Feedback>();

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
            modelBuilder.ApplyConfiguration(new FeedbackConfiguration());
        }

        /// <summary>
        /// Dispatch les domain events avant de sauvegarder.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                    entry.Entity.SetCreatedAudit(now);
                else if (entry.State == EntityState.Modified)
                    entry.Entity.SetUpdatedAudit(now);
            }

            foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.IncrementVersionAudit();
            }

            var domainEvents = ChangeTracker.Entries<AggregateRoot>()
                .SelectMany(e => e.Entity.DomainEvents)
                .ToList();

            var result = await base.SaveChangesAsync(cancellationToken);

            foreach (var entry in ChangeTracker.Entries<AggregateRoot>())
                entry.Entity.ClearDomainEvents();

            return result;
        }
    }


}
