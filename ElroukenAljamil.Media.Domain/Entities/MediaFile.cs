using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Media.Domain.Enums;
using ElroukenAljamil.Media.Domain.ValueObjects;

namespace ElroukenAljamil.Media.Domain.Entities
{
    /// <summary>
    /// Agrégat racine représentant un fichier média (image) sur la marketplace.
    /// Stocke les métadonnées, pas le fichier binaire (celui-ci est dans MinIO).
    /// </summary>
    public class MediaFile : AggregateRoot
    {
        public string OriginalFileName { get; private set; } = string.Empty;
        public string StoragePath { get; private set; } = string.Empty;
        public string ContentType { get; private set; } = string.Empty;
        public long FileSize { get; private set; }
        public ImageDimensions Dimensions { get; private set; } = null!;
        public MediaStatus Status { get; private set; } = MediaStatus.Uploaded;
        public Guid OwnerId { get; private set; }
        public Guid? ListingId { get; private set; }
        public string BucketName { get; private set; } = string.Empty;
        public MediaVariants Variants { get; private set; } = null!;
        public int SortOrder { get; private set; }

        private MediaFile() { } // EF Core

        /// <summary>
        /// Factory method pour créer un fichier média après upload.
        /// </summary>
        public static MediaFile Create(
            string originalFileName,
            string storagePath,
            string contentType,
            long fileSize,
            int width,
            int height,
            Guid ownerId,
            string bucketName)
        {
            // Validation
            if (string.IsNullOrWhiteSpace(originalFileName))
                throw new ArgumentException("Le nom de fichier est obligatoire.", nameof(originalFileName));
            if (fileSize <= 0)
                throw new ArgumentException("La taille du fichier doit être positive.", nameof(fileSize));
            if (fileSize > 10 * 1024 * 1024) // 10 MB max
                throw new ArgumentException("La taille maximale est de 10 MB.", nameof(fileSize));
            if (ownerId == Guid.Empty)
                throw new ArgumentException("Le propriétaire est obligatoire.", nameof(ownerId));

            var allowedContentTypes = new[]
            {
            "image/jpeg", "image/png", "image/webp", "image/gif", "image/avif"
        };
            if (!allowedContentTypes.Contains(contentType.ToLowerInvariant()))
                throw new ArgumentException($"Type de fichier non autorisé : {contentType}");

            var mediaFile = new MediaFile
            {
                Id = Guid.NewGuid(),
                OriginalFileName = SanitizeFileName(originalFileName),
                StoragePath = storagePath,
                ContentType = contentType.ToLowerInvariant(),
                FileSize = fileSize,
                Dimensions = new ImageDimensions(width, height),
                OwnerId = ownerId,
                BucketName = bucketName,
                Status = MediaStatus.Uploaded,
                Variants = MediaVariants.Empty(),
                SortOrder = 0,
                CreatedAt = DateTime.UtcNow
            };

            mediaFile.AddDomainEvent(new MediaUploadedDomainEvent(
                mediaFile.Id, mediaFile.OwnerId, mediaFile.StoragePath));

            return mediaFile;
        }

        /// <summary>
        /// Marque le traitement (redimensionnement, compression) comme terminé.
        /// </summary>
        public void MarkAsProcessed(MediaVariants variants)
        {
            if (Status != MediaStatus.Uploaded && Status != MediaStatus.Processing)
                throw new InvalidOperationException("Seul un fichier uploadé ou en traitement peut être marqué comme traité.");

            Variants = variants ?? throw new ArgumentNullException(nameof(variants));
            Status = MediaStatus.Processed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaProcessedDomainEvent(Id, OwnerId, variants));
        }

        /// <summary>
        /// Marque le fichier comme en cours de traitement.
        /// </summary>
        public void StartProcessing()
        {
            if (Status != MediaStatus.Uploaded)
                throw new InvalidOperationException("Seul un fichier uploadé peut démarrer le traitement.");

            Status = MediaStatus.Processing;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marque le traitement comme échoué.
        /// </summary>
        public void MarkProcessingFailed(string reason)
        {
            Status = MediaStatus.Failed;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaProcessingFailedDomainEvent(Id, OwnerId, reason));
        }

        /// <summary>
        /// Assigne le média à une annonce.
        /// </summary>
        public void AssignToListing(Guid listingId, int sortOrder)
        {
            if (listingId == Guid.Empty)
                throw new ArgumentException("L'identifiant de l'annonce est obligatoire.", nameof(listingId));

            ListingId = listingId;
            SortOrder = sortOrder;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaAssignedToListingDomainEvent(Id, listingId, OwnerId));
        }

        /// <summary>
        /// Détache le média d'une annonce (devient orphelin).
        /// </summary>
        public void DetachFromListing()
        {
            ListingId = null;
            SortOrder = 0;
            UpdatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Marque le média pour suppression.
        /// </summary>
        public void MarkForDeletion()
        {
            Status = MediaStatus.MarkedForDeletion;
            UpdatedAt = DateTime.UtcNow;

            AddDomainEvent(new MediaMarkedForDeletionDomainEvent(Id, OwnerId, StoragePath, BucketName));
        }

        /// <summary>
        /// Vérifie que l'utilisateur est le propriétaire du fichier.
        /// </summary>
        public bool IsOwnedBy(Guid userId) => OwnerId == userId;

        /// <summary>
        /// Vérifie si le média est orphelin (non assigné à une annonce).
        /// </summary>
        public bool IsOrphan => ListingId is null;

        /// <summary>
        /// URL publique de l'image originale.
        /// </summary>
        public string GetPublicUrl(string baseUrl) => $"{baseUrl}/{BucketName}/{StoragePath}";

        /// <summary>
        /// Nettoie le nom de fichier pour éviter les path traversal.
        /// </summary>
        private static string SanitizeFileName(string fileName)
        {
            var sanitized = Path.GetFileName(fileName); // Supprime les chemins
            var invalidChars = Path.GetInvalidFileNameChars();
            sanitized = new string(sanitized.Where(c => !invalidChars.Contains(c)).ToArray());
            return string.IsNullOrWhiteSpace(sanitized) ? "unnamed" : sanitized;
        }
    }
}
