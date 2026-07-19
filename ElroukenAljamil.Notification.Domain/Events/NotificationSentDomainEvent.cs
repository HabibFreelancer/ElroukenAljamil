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
    /// Domain Event émis quand une notification est envoyée avec succès au provider.
    /// 
    /// Déclenché par : NotificationRecord.MarkAsSent()
    /// 
    /// Utilité :
    /// - Métriques : le DeliveryMetricsService enregistre le succès + temps de délivrance
    /// - SignalR : peut déclencher une mise à jour du badge de notifications côté client
    /// - Audit : historique des envois réussis
    /// 
    /// Cet événement confirme que le provider (Brevo, Firebase) a accepté le message.
    /// Cela ne garantit pas que l'utilisateur l'a reçu (ex: email en spam),
    /// mais confirme que notre système a fait son travail correctement.
    /// </summary>
    public record NotificationSentDomainEvent(
        /// <summary>
        /// Identifiant de la notification envoyée.
        /// </summary>
        Guid NotificationId,

        /// <summary>
        /// Identifiant du destinataire.
        /// </summary>
        Guid RecipientId,

        /// <summary>
        /// Canal utilisé pour l'envoi.
        /// </summary>
        NotificationChannel Channel
    ) : BaseDomainEvent;
}
