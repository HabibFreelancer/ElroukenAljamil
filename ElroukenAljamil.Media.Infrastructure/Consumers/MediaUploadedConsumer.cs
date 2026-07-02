using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Media;
using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Interfaces;
using ElroukenAljamil.Media.Domain.ValueObjects;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Media.Infrastructure.Consumers
{
    /// <summary>
    /// Consumer qui traite les images après upload (redimensionnement + variantes WebP).
    /// Déclenché par l'événement MediaUploadedEvent publié lors de l'upload.
    /// </summary>
    public class MediaUploadedConsumer : IConsumer<MediaUploadedEvent>
    {
        private readonly IMediaFileRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ILogger<MediaUploadedConsumer> _logger;

        public MediaUploadedConsumer(
            IMediaFileRepository repository,
            IFileStorageService storageService,
            IImageProcessingService imageProcessingService,
            ILogger<MediaUploadedConsumer> logger)
        {
            _repository = repository;
            _storageService = storageService;
            _imageProcessingService = imageProcessingService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<MediaUploadedEvent> context)
        {
            var message = context.Message;
            _logger.LogInformation("Traitement de l'image {MediaId}...", message.MediaId);

            var mediaFile = await _repository.GetByIdAsync(message.MediaId, context.CancellationToken);
            if (mediaFile is null)
            {
                _logger.LogWarning("Média {MediaId} introuvable, abandon.", message.MediaId);
                return;
            }

            try
            {
                mediaFile.StartProcessing();
                await _repository.UpdateAsync(mediaFile, context.CancellationToken);

                // Télécharger l'original depuis MinIO
                using var originalStream = await _storageService.DownloadAsync(
                    message.BucketName, message.StoragePath, context.CancellationToken);

                // Générer les variantes
                var variants = await _imageProcessingService.GenerateVariantsAsync(
                    originalStream, context.CancellationToken);

                // Upload des variantes vers MinIO
                var basePath = Path.GetDirectoryName(message.StoragePath)?.Replace("\\", "/") ?? "";
                var fileId = Path.GetFileNameWithoutExtension(message.StoragePath);

                var thumbnailPath = $"{basePath}/{fileId}_thumb.webp";
                var mediumPath = $"{basePath}/{fileId}_medium.webp";
                var largePath = $"{basePath}/{fileId}_large.webp";
                var webpPath = $"{basePath}/{fileId}_full.webp";

                await _storageService.UploadAsync(
                    message.BucketName, thumbnailPath,
                    variants.Thumbnail.OutputStream, "image/webp", context.CancellationToken);

                await _storageService.UploadAsync(
                    message.BucketName, mediumPath,
                    variants.Medium.OutputStream, "image/webp", context.CancellationToken);

                await _storageService.UploadAsync(
                    message.BucketName, largePath,
                    variants.Large.OutputStream, "image/webp", context.CancellationToken);

                await _storageService.UploadAsync(
                    message.BucketName, webpPath,
                    variants.WebP.OutputStream, "image/webp", context.CancellationToken);

                // Mettre à jour l'entité avec les chemins des variantes
                var mediaVariants = new MediaVariants
                {
                    ThumbnailPath = thumbnailPath,
                    MediumPath = mediumPath,
                    LargePath = largePath,
                    WebPPath = webpPath
                };

                mediaFile.MarkAsProcessed(mediaVariants);
                await _repository.UpdateAsync(mediaFile, context.CancellationToken);

                _logger.LogInformation("Image {MediaId} traitée avec succès.", message.MediaId);

                // Dispose des streams
                variants.Thumbnail.OutputStream.Dispose();
                variants.Medium.OutputStream.Dispose();
                variants.Large.OutputStream.Dispose();
                variants.WebP.OutputStream.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du traitement de l'image {MediaId}.", message.MediaId);
                mediaFile.MarkProcessingFailed(ex.Message);
                await _repository.UpdateAsync(mediaFile, context.CancellationToken);
            }
        }
    }

}
