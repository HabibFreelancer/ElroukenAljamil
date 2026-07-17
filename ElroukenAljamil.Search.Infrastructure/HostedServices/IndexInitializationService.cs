using ElroukenAljamil.Search.Domain.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Search.Infrastructure.HostedServices
{
    /// <summary>
    /// Service de démarrage qui crée l'index Elasticsearch au lancement de l'application.
    /// </summary>
    public class IndexInitializationService : IHostedService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<IndexInitializationService> _logger;

        public IndexInitializationService(
            IServiceScopeFactory scopeFactory,
            ILogger<IndexInitializationService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Vérification/création de l'index Elasticsearch...");

            // Attendre qu'Elasticsearch soit disponible (retry en boucle)
            var maxRetries = 10;
            var delay = TimeSpan.FromSeconds(5);

            for (var i = 0; i < maxRetries; i++)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var indexManagement = scope.ServiceProvider.GetRequiredService<IIndexManagementService>();

                    if (!await indexManagement.IndexExistsAsync(cancellationToken))
                    {
                        await indexManagement.CreateIndexAsync(cancellationToken);
                        _logger.LogInformation("Index Elasticsearch créé au démarrage.");
                    }
                    else
                    {
                        _logger.LogInformation("Index Elasticsearch déjà existant.");
                    }

                    return;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(
                        "Elasticsearch non disponible (tentative {Attempt}/{Max}): {Error}",
                        i + 1, maxRetries, ex.Message);
                    await Task.Delay(delay, cancellationToken);
                }
            }

            _logger.LogError("Impossible de se connecter à Elasticsearch après {Max} tentatives.", maxRetries);
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
