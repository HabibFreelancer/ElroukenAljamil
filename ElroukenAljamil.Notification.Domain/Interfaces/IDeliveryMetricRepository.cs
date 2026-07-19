using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    /// <summary>
    /// Repository pour les métriques de délivrance.
    /// </summary>
    public interface IDeliveryMetricRepository : IRepository<DeliveryMetric>
    {
        Task<DeliveryMetric?> GetOrCreateForPeriodAsync(
            NotificationChannel channel, NotificationType type,
            DateTime periodStart, CancellationToken ct = default);

        Task<IReadOnlyList<DeliveryMetric>> GetByPeriodAsync(
            DateTime from, DateTime to, CancellationToken ct = default);

        Task<IReadOnlyList<DeliveryMetric>> GetByChannelAndPeriodAsync(
            NotificationChannel channel, DateTime from, DateTime to,
            CancellationToken ct = default);
    }
}
