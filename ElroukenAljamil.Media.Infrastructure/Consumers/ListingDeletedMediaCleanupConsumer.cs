using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings;
using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Interfaces;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Media.Infrastructure.Consumers
{
    /// <summary>
    /// Consumer qui nettoie les images lorsqu'une annonce est supprimée/désactivée.
    /// </summary>
    public class ListingDeletedMediaCleanupConsumer : IConsumer<ListingDeactivatedEvent>
    {
        private readonly IMediaFileRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly ILogger<ListingDeletedMediaCleanupConsumer> _logger;

        public ListingDeletedMediaCleanupConsumer(
            IMediaFileRepository repository,
            IFileStorageService storageService,
            ILogger<ListingDeletedMediaCleanupConsumer> logger)
        {
            _repository = repository;
            _storageService = storageService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<ListingDeactivatedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation(
                "Nettoyage des médias pour l'annonce désactivée {ListingId}.", message.ListingId);

            var mediaFiles = await _repository.GetByListingIdAsync(
                message.ListingId, context.CancellationToken);

            foreach (var mediaFile in mediaFiles)
            {
                try
                {
                    // Supprimer tous les fichiers de MinIO
                    await _storageService.DeleteAsync(
                        mediaFile.BucketName, mediaFile.StoragePath, context.CancellationToken);

                    if (mediaFile.Variants.ThumbnailPath is not null)
                        await _storageService.DeleteAsync(
                            mediaFile.BucketName, mediaFile.Variants.ThumbnailPath, context.CancellationToken);
                    if (mediaFile.Variants.MediumPath is not null)
                        await _storageService.DeleteAsync(
                            mediaFile.BucketName, mediaFile.Variants.MediumPath, context.CancellationToken);
                    if (mediaFile.Variants.LargePath is not null)
                        await _storageService.DeleteAsync(
                            mediaFile.BucketName, mediaFile.Variants.LargePath, context.CancellationToken);
                    if (mediaFile.Variants.WebPPath is not null)
                        await _storageService.DeleteAsync(
                            mediaFile.BucketName, mediaFile.Variants.WebPPath, context.CancellationToken);

                    mediaFile.MarkForDeletion();
                    await _repository.DeleteAsync(mediaFile, context.CancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors de la suppression du média {MediaId}.", mediaFile.Id);
                }
            }

            _logger.LogInformation(
                "{Count} médias nettoyés pour l'annonce {ListingId}.",
                mediaFiles.Count, message.ListingId);
        }
    }
}
