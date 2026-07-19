using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Notification.Domain.Enums;

namespace ElroukenAljamil.Notification.Domain.Events
{
    /// <summary>
    /// Domain Event émis quand une notification est créée et mise en file d'attente.
    /// 
    /// Déclenché par : NotificationRecord.Create()
    /// 
    /// Utilité :
    /// - Traçabilité : permet de logger chaque notification créée
    /// - Métriques : le DeliveryMetricsService peut s'y abonner pour compter les tentatives
    /// - Extensibilité : d'autres handlers internes pourraient réagir (ex: rate limiting par utilisateur)
    /// 
    /// Note : ce sont des Domain Events (internes au microservice), pas des Integration Events.
    /// Ils ne sont PAS publiés sur RabbitMQ. Ils sont dispatchés via MediatR en mémoire.
    /// </summary>
    public record NotificationCreatedDomainEvent(
        /// <summary>
        /// Identifiant de la notification créée.
        /// </summary>
        Guid NotificationId,

        /// <summary>
        /// Identifiant du destinataire.
        /// </summary>
        Guid RecipientId,

        /// <summary>
        /// Type métier de la notification (NewMessage, ListingPublished, etc.).
        /// </summary>
        NotificationType Type,

        /// <summary>
        /// Canal sur lequel la notification sera envoyée.
        /// </summary>
        NotificationChannel Channel
    ) : BaseDomainEvent;

}
