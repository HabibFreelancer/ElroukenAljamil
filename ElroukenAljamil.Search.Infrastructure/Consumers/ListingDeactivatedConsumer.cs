using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.Search.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Search.Infrastructure.Consumers
{
    /// <summary>
    /// Supprime l'annonce de l'index quand elle est désactivée.
    /// </summary>
    public class ListingDeactivatedConsumer : IConsumer<ListingDeactivatedEvent>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<ListingDeactivatedConsumer> _logger;

        public ListingDeactivatedConsumer(
            ISearchRepository searchRepository,
            ILogger<ListingDeactivatedConsumer> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ListingDeactivatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Suppression de l'annonce {ListingId} de l'index.", message.ListingId);

            var guid = new Guid(message.ListingId, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));
            await _searchRepository.DeleteAsync(guid, cts.Token);
            _logger.LogInformation("Annonce {ListingId} supprimée de l'index.", message.ListingId);
        }
    }
}
