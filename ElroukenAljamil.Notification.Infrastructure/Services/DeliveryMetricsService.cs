using System.Linq;
using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.Infrastructure.Services
{
    /// <summary>
    /// Service qui enregistre et agrège les métriques de délivrance.
    /// Chaque envoi (réussi ou échoué) est comptabilisé dans une entrée horaire.
    /// Le tableau de bord admin interroge ce service pour afficher les KPIs.
    /// </summary>
    public class DeliveryMetricsService : IDeliveryMetricsService
    {
        private readonly IDeliveryMetricRepository _metricRepository;
        private readonly ILogger<DeliveryMetricsService> _logger;

        public DeliveryMetricsService(
            IDeliveryMetricRepository metricRepository,
            ILogger<DeliveryMetricsService> logger)
        {
            _metricRepository = metricRepository;
            _logger = logger;
        }

        public async Task RecordSuccessAsync(
            NotificationChannel channel, NotificationType type,
            double deliveryTimeMs, CancellationToken ct = default)
        {
            var metric = await _metricRepository.GetOrCreateForPeriodAsync(
                channel, type, DateTime.UtcNow, ct);

            metric!.RecordSuccess(deliveryTimeMs);
            await _metricRepository.UpdateAsync(metric, ct);
        }

        public async Task RecordFailureAsync(
            NotificationChannel channel, NotificationType type,
            CancellationToken ct = default)
        {
            var metric = await _metricRepository.GetOrCreateForPeriodAsync(
                channel, type, DateTime.UtcNow, ct);

            metric!.RecordFailure();
            await _metricRepository.UpdateAsync(metric, ct);
        }

        public async Task<DashboardMetrics> GetDashboardMetricsAsync(
            DateTime from, DateTime to, CancellationToken ct = default)
        {
            var metrics = await _metricRepository.GetByPeriodAsync(from, to, ct);

            var totalSent = metrics.Sum(m => m.SuccessCount);
            var totalFailed = metrics.Sum(m => m.FailureCount);
            var totalAttempts = metrics.Sum(m => m.TotalAttempts);

            // Métriques par canal
            var byChannel = metrics
                .GroupBy(m => m.Channel)
                .Select(g => new ChannelMetrics
                {
                    Channel = g.Key.ToString(),
                    Sent = g.Sum(m => m.SuccessCount),
                    Failed = g.Sum(m => m.FailureCount),
                    SuccessRate = g.Sum(m => m.TotalAttempts) > 0
                        ? (double)g.Sum(m => m.SuccessCount) / g.Sum(m => m.TotalAttempts) * 100
                        : 0,
                    AvgDeliveryMs = g.Where(m => m.SuccessCount > 0).Any()
                        ? g.Where(m => m.SuccessCount > 0).Average(m => m.AverageDeliveryTimeMs)
                        : 0
                })
                .ToList();

            // Métriques horaires (pour le graphique)
            var hourly = metrics
                .GroupBy(m => m.PeriodStart)
                .OrderBy(g => g.Key)
                .Select(g => new HourlyMetrics
                {
                    Hour = g.Key,
                    Sent = g.Sum(m => m.SuccessCount),
                    Failed = g.Sum(m => m.FailureCount)
                })
                .ToList();

            return new DashboardMetrics
            {
                TotalSent = totalSent,
                TotalFailed = totalFailed,
                OverallSuccessRate = totalAttempts > 0 ? (double)totalSent / totalAttempts * 100 : 0,
                AverageDeliveryTimeMs = metrics.Where(m => m.SuccessCount > 0).Any()
                    ? metrics.Where(m => m.SuccessCount > 0).Average(m => m.AverageDeliveryTimeMs)
                    : 0,
                ByChannel = byChannel,
                Hourly = hourly
            };
        }
    }

}
