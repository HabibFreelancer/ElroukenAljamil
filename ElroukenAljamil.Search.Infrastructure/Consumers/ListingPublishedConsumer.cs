using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.Search.Domain.Entities;
using ElroukenAljamil.Search.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Search.Infrastructure.Consumers
{
    /// <summary>
    /// Indexe une nouvelle annonce dans Elasticsearch quand elle est publiée.
    /// </summary>
    public class ListingPublishedConsumer : IConsumer<ListingPublishedEvent>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly IIndexManagementService _indexManagement;
        private readonly ILogger<ListingPublishedConsumer> _logger;

        public ListingPublishedConsumer(
            ISearchRepository searchRepository,
            IIndexManagementService indexManagement,
            ILogger<ListingPublishedConsumer> logger)
        {
            _searchRepository = searchRepository;
            _indexManagement = indexManagement;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ListingPublishedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Indexation de l'annonce {ListingId}: {Title}", message.ListingId, message.Title);

            // S'assurer que l'index existe
            if (!await _indexManagement.IndexExistsAsync(context.CancellationToken))
            {
                await _indexManagement.CreateIndexAsync(context.CancellationToken);
            }

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
                SellerId = message.SellerId,
                SellerName = message.SellerName,
                ImageUrls = message.ImageUrls,
                ThumbnailUrl = message.ImageUrls.FirstOrDefault(),
                Status = "Active",
                PublishedAt = message.PublishedAt
            };

            await _searchRepository.IndexAsync(document, context.CancellationToken);
            _logger.LogInformation("Annonce {ListingId} indexée avec succès.", message.ListingId);
        }
    }
}
