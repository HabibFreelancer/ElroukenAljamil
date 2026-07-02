using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.Media
{
    /// <summary>
    /// Événement publié lorsqu'une image est uploadée avec succès sur MinIO.
    /// Consommé par le Media.Service lui-même (consumer interne) pour déclencher
    /// le traitement asynchrone (redimensionnement, génération des variantes WebP).
    /// </summary>
    public record MediaUploadedEvent : IntegrationEvent
    {
        /// <summary>
        /// Identifiant du fichier média en base.
        /// </summary>
        public Guid MediaId { get; init; }

        /// <summary>
        /// Propriétaire du fichier (utilisateur qui a uploadé).
        /// </summary>
        public Guid OwnerId { get; init; }

        /// <summary>
        /// Chemin de stockage dans MinIO (ex: "userId/2026/07/guid.jpg").
        /// </summary>
        public string StoragePath { get; init; } = string.Empty;

        /// <summary>
        /// Nom du bucket MinIO.
        /// </summary>
        public string BucketName { get; init; } = string.Empty;

        /// <summary>
        /// Type MIME du fichier (ex: "image/jpeg").
        /// </summary>
        public string ContentType { get; init; } = string.Empty;

        /// <summary>
        /// Date de l'upload.
        /// </summary>
        public DateTime UploadedAt { get; init; }
    }
}
