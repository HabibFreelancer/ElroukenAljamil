using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Events
{
    /// <summary>
    /// Domain Event émis quand une notification échoue définitivement (après toutes les tentatives).
    /// 
    /// Déclenché par : NotificationRecord.MarkAsFailed() quand RetryCount >= MaxRetries (3)
    /// 
    /// Utilité :
    /// - Alerting : peut déclencher une alerte admin si le taux d'échec dépasse un seuil
    /// - Métriques : le DeliveryMetricsService enregistre l'échec
    /// - Monitoring : permet de détecter un problème de configuration (clé API expirée, SMTP down)
    /// - Fallback : pourrait déclencher un envoi sur un canal alternatif
    /// 
    /// Important : cet événement n'est émis qu'après l'échec DÉFINITIF (3 tentatives).
    /// Les échecs intermédiaires (tentative 1 et 2) ne déclenchent pas cet événement —
    /// la notification reste en Pending et sera retentée par le NotificationRetryWorker.
    /// </summary>
    public record NotificationFailedDomainEvent(
        /// <summary>
        /// Identifiant de la notification en échec.
        /// </summary>
        Guid NotificationId,

        /// <summary>
        /// Identifiant du destinataire qui n'a pas reçu la notification.
        /// </summary>
        Guid RecipientId,

        /// <summary>
        /// Canal sur lequel l'envoi a échoué.
        /// </summary>
        NotificationChannel Channel,

        /// <summary>
        /// Message d'erreur technique (ex: "SMTP connection refused", "Invalid API key").
        /// </summary>
        string ErrorMessage
    ) : BaseDomainEvent;
}
