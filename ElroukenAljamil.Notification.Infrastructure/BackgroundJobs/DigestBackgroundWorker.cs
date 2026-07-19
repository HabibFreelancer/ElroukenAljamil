using ElroukenAljamil.Notification.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.BackgroundJobs
{
    /// <summary>
    /// Worker qui vérifie toutes les heures s'il y a des digests à envoyer.
    /// Il délègue au DigestService la logique de vérification et d'envoi.
    /// </summary>
    public class DigestBackgroundWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<DigestBackgroundWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

        public DigestBackgroundWorker(
            IServiceScopeFactory scopeFactory,
            ILogger<DigestBackgroundWorker> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("DigestBackgroundWorker démarré. Intervalle : {Interval}.", _checkInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var digestService = scope.ServiceProvider.GetRequiredService<IDigestService>();
                    await digestService.ProcessPendingDigestsAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur dans le DigestBackgroundWorker.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }
        }
    }
}
