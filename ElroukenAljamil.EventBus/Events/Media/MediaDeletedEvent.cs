using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.Media
{
    /// <summary>
    /// Événement publié lorsqu'un média est supprimé.
    /// Consommé par :
    /// - Listings.Service : pour retirer l'URL de l'image de l'annonce associée
    /// - Notification.Service : pour informer le vendeur si la suppression est automatique
    /// - Search.Service : pour mettre à jour l'index (retirer l'image des résultats)
    /// </summary>
    public record MediaDeletedEvent : IntegrationEvent
    {
        /// <summary>
        /// Identifiant du média supprimé.
        /// </summary>
        public Guid MediaId { get; init; }

        /// <summary>
        /// Annonce associée (null si le média était orphelin).
        /// </summary>
        public Guid? ListingId { get; init; }

        /// <summary>
        /// Propriétaire du fichier.
        /// </summary>
        public Guid OwnerId { get; init; }

        /// <summary>
        /// Date de la suppression.
        /// </summary>
        public DateTime DeletedAt { get; init; }
    }
}
