using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Entities
{
    /// <summary>
    /// Entité qui enregistre les métriques de délivrance des notifications.
    /// Agrégée par heure pour le tableau de bord admin.
    /// Permet de monitorer les taux de succès/échec par canal et par type.
    /// </summary>
    public class DeliveryMetric : BaseEntity
    {
        /// <summary>
        /// Canal de notification mesuré.
        /// </summary>
        public NotificationChannel Channel { get; private set; }

        /// <summary>
        /// Type de notification mesuré.
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Début de la période mesurée (arrondi à l'heure).
        /// </summary>
        public DateTime PeriodStart { get; private set; }

        /// <summary>
        /// Nombre total de notifications tentées dans cette période.
        /// </summary>
        public int TotalAttempts { get; private set; }

        /// <summary>
        /// Nombre de notifications envoyées avec succès.
        /// </summary>
        public int SuccessCount { get; private set; }

        /// <summary>
        /// Nombre de notifications en échec.
        /// </summary>
        public int FailureCount { get; private set; }

        /// <summary>
        /// Temps de délivrance moyen en millisecondes.
        /// </summary>
        public double AverageDeliveryTimeMs { get; private set; }

        /// <summary>
        /// Taux de succès (0.0 à 1.0).
        /// </summary>
        public double SuccessRate => TotalAttempts > 0 ? (double)SuccessCount / TotalAttempts : 0;

        private DeliveryMetric() { } // EF Core

        /// <summary>
        /// Crée une nouvelle entrée de métrique pour une période donnée.
        /// </summary>
        public static DeliveryMetric Create(
            NotificationChannel channel,
            NotificationType type,
            DateTime periodStart)
        {
            return new DeliveryMetric
            {
                Id = Guid.NewGuid(),
                Channel = channel,
                Type = type,
                PeriodStart = new DateTime(periodStart.Year, periodStart.Month, periodStart.Day,
                                           periodStart.Hour, 0, 0, DateTimeKind.Utc),
                TotalAttempts = 0,
                SuccessCount = 0,
                FailureCount = 0,
                AverageDeliveryTimeMs = 0,
                CreatedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Enregistre un envoi réussi.
        /// </summary>
        public void RecordSuccess(double deliveryTimeMs)
        {
            TotalAttempts++;
            SuccessCount++;

            // Calcul de la moyenne mobile
            AverageDeliveryTimeMs = ((AverageDeliveryTimeMs * (SuccessCount - 1)) + deliveryTimeMs) / SuccessCount;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Enregistre un envoi échoué.
        /// </summary>
        public void RecordFailure()
        {
            TotalAttempts++;
            FailureCount++;
            UpdatedAt = DateTime.UtcNow;
        }
    }

}
