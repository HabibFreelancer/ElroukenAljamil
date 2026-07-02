using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.Media
{
    public record MediaProcessingCompletedEvent : IntegrationEvent
    {
        public Guid MediaId { get; init; }
        public Guid OwnerId { get; init; }
        public Guid? ListingId { get; init; }
        public string OriginalUrl { get; init; } = string.Empty;
        public string ThumbnailUrl { get; init; } = string.Empty;
        public string MediumUrl { get; init; } = string.Empty;
        public string LargeUrl { get; init; } = string.Empty;
    }

    /// <summary>
    /// Événement publié lorsque le traitement d'une image échoue.
    /// Consommé par :
    /// - Notification.Service : pour alerter le vendeur que son image n'a pas pu être traitée
    /// </summary>
    public record MediaProcessingFailedEvent : IntegrationEvent
    {
        /// <summary>
        /// Identifiant du média en échec.
        /// </summary>
        public Guid MediaId { get; init; }

        /// <summary>
        /// Propriétaire du fichier.
        /// </summary>
        public Guid OwnerId { get; init; }

        /// <summary>
        /// Nom du fichier original (pour l'afficher dans la notification).
        /// </summary>
        public string OriginalFileName { get; init; } = string.Empty;

        /// <summary>
        /// Raison de l'échec.
        /// </summary>
        public string FailureReason { get; init; } = string.Empty;

        /// <summary>
        /// Date de l'échec.
        /// </summary>
        public DateTime FailedAt { get; init; }
    }
}
