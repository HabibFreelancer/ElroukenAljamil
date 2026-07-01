using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.Listings.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Worker qui vérifie périodiquement les annonces expirées et met à jour leur statut.
    /// </summary>
    public class ListingExpirationWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ListingExpirationWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ListingExpirationWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<ListingExpirationWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ListingExpirationWorker démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CheckExpiredListingsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la vérification des annonces expirées.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CheckExpiredListingsAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IListingRepository>();
            var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();

            var expiredListings = await repository.GetExpiredListingsAsync(ct);

            foreach (var listing in expiredListings)
            {
                listing.CheckExpiration();
                await repository.UpdateAsync(listing, ct);

                await eventBus.PublishAsync(new ListingDeactivatedEvent
                {
                    ListingId = listing.Id,
                    SellerId = listing.SellerId,
                    DeactivatedAt = DateTime.UtcNow
                }, ct);

                _logger.LogInformation("Annonce {ListingId} marquée comme expirée.", listing.Id);
            }

            if (expiredListings.Any())
                _logger.LogInformation("{Count} annonces expirées traitées.", expiredListings.Count);
        }
    }

}
