using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Application.Interfaces
{
    /// <summary>
    /// Service d'enregistrement et d'agrégation des métriques de délivrance.
    /// Appelé après chaque tentative d'envoi (succès ou échec).
    /// </summary>
    public interface IDeliveryMetricsService
    {
        /// <summary>
        /// Enregistre un envoi réussi.
        /// </summary>
        Task RecordSuccessAsync(
            NotificationChannel channel, NotificationType type,
            double deliveryTimeMs, CancellationToken ct = default);

        /// <summary>
        /// Enregistre un envoi échoué.
        /// </summary>
        Task RecordFailureAsync(
            NotificationChannel channel, NotificationType type,
            CancellationToken ct = default);

        /// <summary>
        /// Récupère les métriques agrégées pour une période.
        /// </summary>
        Task<DashboardMetrics> GetDashboardMetricsAsync(
            DateTime from, DateTime to, CancellationToken ct = default);
    }

    /// <summary>
    /// Métriques agrégées pour le tableau de bord admin.
    /// </summary>
    public record DashboardMetrics
    {
        public int TotalSent { get; init; }
        public int TotalFailed { get; init; }
        public double OverallSuccessRate { get; init; }
        public double AverageDeliveryTimeMs { get; init; }
        public List<ChannelMetrics> ByChannel { get; init; } = new();
        public List<HourlyMetrics> Hourly { get; init; } = new();
    }

    public record ChannelMetrics
    {
        public string Channel { get; init; } = string.Empty;
        public int Sent { get; init; }
        public int Failed { get; init; }
        public double SuccessRate { get; init; }
        public double AvgDeliveryMs { get; init; }
    }

    public record HourlyMetrics
    {
        public DateTime Hour { get; init; }
        public int Sent { get; init; }
        public int Failed { get; init; }
    }


}
