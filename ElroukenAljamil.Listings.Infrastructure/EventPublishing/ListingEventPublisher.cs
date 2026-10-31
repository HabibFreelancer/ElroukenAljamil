using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.EventPublishing
{
    /// <summary>
    /// Publie les événements d'intégration vers RabbitMQ.
    /// Appelé après persistance en base.
    /// </summary>
    public class ListingIntegrationEventPublisher
    {
        private readonly IEventBus _eventBus;
        private readonly ILogger<ListingIntegrationEventPublisher> _logger;

        public ListingIntegrationEventPublisher(IEventBus eventBus, ILogger<ListingIntegrationEventPublisher> logger)
        {
            _eventBus = eventBus;
            _logger = logger;
        }

        /// <summary>Convertit un int Id en Guid déterministe pour Elasticsearch.</summary>
        public static Guid ToGuid(int id) =>
            new Guid(id, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);

        public async Task PublishListingPublishedAsync(
            int listingId, string title, string description, decimal price, string category,
            string sellerName, Guid sellerId, string city, List<string> imageUrls,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Publication event ListingPublished pour annonce {ListingId} — {Title}", listingId, title);
            try
            {
                await _eventBus.PublishAsync(new ListingPublishedEvent
                {
                    ListingId   = ToGuid(listingId),
                    Title       = title,
                    Description = description,
                    Price       = price,
                    Category    = category,
                    SellerName  = sellerName,
                    SellerId    = sellerId,
                    City        = city,
                    ImageUrls   = imageUrls,
                    Currency    = "TND",
                    PublishedAt = DateTime.UtcNow
                }, ct);
                _logger.LogInformation("Event ListingPublished publié avec succès pour annonce {ListingId}", listingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec publication event ListingPublished pour annonce {ListingId}", listingId);
            }
        }

        public async Task PublishListingUpdatedAsync(
            int listingId, string title, string description, decimal price, string category,
            string city, List<string> imageUrls,
            CancellationToken ct = default)
        {
            _logger.LogInformation("Publication event ListingUpdated pour annonce {ListingId}", listingId);
            try
            {
                await _eventBus.PublishAsync(new ListingUpdatedEvent
                {
                    ListingId   = ToGuid(listingId),
                    Title       = title,
                    Description = description,
                    Price       = price,
                    Category    = category,
                    City        = city,
                    ImageUrls   = imageUrls,
                    Currency    = "TND",
                    UpdatedAt   = DateTime.UtcNow
                }, ct);
                _logger.LogInformation("Event ListingUpdated publié avec succès pour annonce {ListingId}", listingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec publication event ListingUpdated pour annonce {ListingId}", listingId);
            }
        }

        public async Task PublishListingDeactivatedAsync(int listingId, CancellationToken ct = default)
        {
            _logger.LogInformation("Publication event ListingDeactivated pour annonce {ListingId}", listingId);
            try
            {
                await _eventBus.PublishAsync(new ListingDeactivatedEvent
                {
                    ListingId      = listingId,
                    DeactivatedAt  = DateTime.UtcNow
                }, ct);
                _logger.LogInformation("Event ListingDeactivated publié avec succès pour annonce {ListingId}", listingId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Échec publication event ListingDeactivated pour annonce {ListingId}", listingId);
            }
        }
    }
}
