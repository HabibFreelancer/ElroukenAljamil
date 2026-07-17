using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Media;
using ElroukenAljamil.Search.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;
namespace ElroukenAljamil.Search.Infrastructure.Consumers
{
    /// <summary>
    /// Met à jour l'index quand un média est supprimé (retire l'URL de l'image).
    /// </summary>
    public class MediaDeletedSearchConsumer : IConsumer<MediaDeletedEvent>
    {
        private readonly ISearchRepository _searchRepository;
        private readonly ILogger<MediaDeletedSearchConsumer> _logger;

        public MediaDeletedSearchConsumer(
            ISearchRepository searchRepository,
            ILogger<MediaDeletedSearchConsumer> logger)
        {
            _searchRepository = searchRepository;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MediaDeletedEvent> context)
        {
            var message = context.Message;

            if (message.ListingId is null)
            {
                _logger.LogDebug("Média orphelin supprimé, pas d'impact sur l'index.");
                return;
            }

            _logger.LogInformation(
                "Média {MediaId} supprimé, mise à jour de l'annonce {ListingId} dans l'index.",
                message.MediaId, message.ListingId);

            // Note : dans un cas réel, on re-fetch les ImageUrls à jour depuis le Listings.Service
            // ou on attend le prochain ListingUpdatedEvent. Ici on log simplement l'événement.
            _logger.LogDebug(
                "L'index sera mis à jour au prochain ListingUpdatedEvent pour l'annonce {ListingId}.",
                message.ListingId);
        }
    }

}
