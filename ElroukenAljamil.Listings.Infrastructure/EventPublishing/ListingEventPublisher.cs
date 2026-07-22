using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;


namespace ElroukenAljamil.Listings.Infrastructure.EventPublishing
{
    /// <summary>
    /// Publie les événements d'intégration vers RabbitMQ via IEventBusPublisher.
    /// Appelé après persistance des domain events.
    /// </summary>
    public class ListingIntegrationEventPublisher
    {
        private readonly IEventBus _eventBus;

        public ListingIntegrationEventPublisher(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        public async Task PublishListingCreatedAsync(Guid listingId, string title, string description, Guid sellerId, string city, double? latitude, double? longitude, List<string> imageUrls, CancellationToken ct = default)
        {
            var integrationEvent = new ListingCreatedIntegrationEvent
            {
                ListingId = listingId,
                Title = title,
                Description = description,
                SellerId = sellerId,
                City = city,
                Latitude = latitude,
                Longitude = longitude,
                ImageUrls = imageUrls,
            };

            await _eventBus.PublishAsync(integrationEvent, ct);
        }

        public async Task PublishListingDeactivatedAsync(Guid listingId, CancellationToken ct = default)
        {
            var integrationEvent = new ListingDeactivatedEvent
            {
                ListingId = listingId,
                // DeactivatedAt = DateTime.UtcNow
            };

            await _eventBus.PublishAsync(integrationEvent, ct);
        }
    }


}
