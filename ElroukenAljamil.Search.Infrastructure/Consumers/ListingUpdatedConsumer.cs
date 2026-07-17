using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.Search.Domain.Entities;
using ElroukenAljamil.Search.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Search.Infrastructure.Consumers
{
    /// <summary>
    /// Met à jour le document dans l'index quand une annonce est modifiée.
    /// </summary>
    public class ListingUpdatedConsumer : IConsumer<ListingUpdatedEvent>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<ListingUpdatedConsumer> _logger;

        public ListingUpdatedConsumer(
            ISearchRepository searchRepository,
            ILogger<ListingUpdatedConsumer> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ListingUpdatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Mise à jour de l'annonce {ListingId} dans l'index.", message.ListingId);

            var document = new SearchableListingDocument
            {
                Id = message.ListingId,
                Title = message.Title,
                Description = message.Description,
                Price = message.Price,
                Currency = message.Currency,
                Category = message.Category,
                City = message.City,
                Latitude = message.Latitude,
                Longitude = message.Longitude,
                ImageUrls = message.ImageUrls,
                ThumbnailUrl = message.ImageUrls.FirstOrDefault(),
                Status = "Active",
                UpdatedAt = message.UpdatedAt
            };

            await _searchRepository.UpdateAsync(document, context.CancellationToken);
            _logger.LogInformation("Annonce {ListingId} mise à jour dans l'index.", message.ListingId);
        }
    }
}
