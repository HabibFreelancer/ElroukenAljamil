using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Worker de vérification des annonces expirées — à brancher sur IAnnonceRepository.
    /// </summary>
    public class ListingExpirationWorker : BackgroundService
    {
        private readonly ILogger<ListingExpirationWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public ListingExpirationWorker(ILogger<ListingExpirationWorker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("ListingExpirationWorker démarré.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Vérification des annonces expirées — à implémenter via IAnnonceRepository.");
                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
