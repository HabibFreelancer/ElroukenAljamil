using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Media.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Worker qui supprime les fichiers orphelins (non assignés à une annonce après 24h).
    /// </summary>
    public class OrphanedMediaCleanupWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OrphanedMediaCleanupWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(6);
        private readonly TimeSpan _orphanThreshold = TimeSpan.FromHours(24);

        public OrphanedMediaCleanupWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<OrphanedMediaCleanupWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("OrphanedMediaCleanupWorker démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await CleanupOrphanedFilesAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du nettoyage des fichiers orphelins.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }

        private async Task CleanupOrphanedFilesAsync(CancellationToken ct)
        {
            using var scope = _scopeFactory.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IMediaFileRepository>();
            var storageService = scope.ServiceProvider.GetRequiredService<IFileStorageService>();

            var orphanedFiles = await repository.GetOrphanedFilesAsync(_orphanThreshold, ct);

            if (!orphanedFiles.Any())
            {
                _logger.LogDebug("Aucun fichier orphelin à nettoyer.");
                return;
            }

            var deletedCount = 0;

            foreach (var mediaFile in orphanedFiles)
            {
                try
                {
                    // Supprimer de MinIO
                    await storageService.DeleteAsync(mediaFile.BucketName, mediaFile.StoragePath, ct);

                    if (mediaFile.Variants.ThumbnailPath is not null)
                        await storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.ThumbnailPath, ct);
                    if (mediaFile.Variants.MediumPath is not null)
                        await storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.MediumPath, ct);
                    if (mediaFile.Variants.LargePath is not null)
                        await storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.LargePath, ct);
                    if (mediaFile.Variants.WebPPath is not null)
                        await storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.WebPPath, ct);

                    // Supprimer de la base
                    await repository.DeleteAsync(mediaFile, ct);
                    deletedCount++;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Impossible de supprimer le fichier orphelin {MediaId}.", mediaFile.Id);
                }
            }

            _logger.LogInformation("{Count} fichiers orphelins supprimés.", deletedCount);
        }
    }
}
