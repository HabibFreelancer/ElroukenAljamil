using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.Listings.Domain.Entities;


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

        public async Task PublishListingCreatedAsync(Listing listing, CancellationToken ct = default)
        {
            var integrationEvent = new ListingCreatedIntegrationEvent
            {
                ListingId = listing.Id,
                Title = listing.Title,
                Description = listing.Description,
                //   Price = listing.Price,
                // Currency = listing.Currency,
                //  CategoryId = listing.CategoryId,
                SellerId = listing.SellerId,
                City = listing.Location.City,
                Latitude = listing.Location.Latitude,
                Longitude = listing.Location.Longitude,
                ImageUrls = listing.ImageUrls,
                //  CreatedAt = listing.CreatedAt
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
