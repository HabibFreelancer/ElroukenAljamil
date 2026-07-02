using ElroukenAljamil.Media.Domain.ValueObjects;

namespace ElroukenAljamil.Media.Application.Interfaces
{
    /// <summary>
    /// Service de traitement d'images (redimensionnement, compression, conversion).
    /// </summary>
    public interface IImageProcessingService
    {
        /// <summary>
        /// Redimensionne une image à la largeur maximale donnée (conserve le ratio).
        /// </summary>
        Task<ImageProcessingResult> ResizeAsync(
            Stream inputStream,
            int maxWidth,
            string outputFormat = "webp",
            int quality = 80,
            CancellationToken ct = default);

        /// <summary>
        /// Génère toutes les variantes d'une image (thumbnail, medium, large, webp).
        /// </summary>
        Task<ImageVariantsResult> GenerateVariantsAsync(
            Stream inputStream,
            CancellationToken ct = default);

        /// <summary>
        /// Lit les dimensions d'une image sans la charger entièrement.
        /// </summary>
        Task<ImageDimensions> GetDimensionsAsync(Stream inputStream, CancellationToken ct = default);

        /// <summary>
        /// Valide qu'un stream est réellement une image valide.
        /// </summary>
        Task<bool> IsValidImageAsync(Stream inputStream, CancellationToken ct = default);
    }

    public record ImageProcessingResult
    {
        public Stream OutputStream { get; init; } = null!;
        public string ContentType { get; init; } = string.Empty;
        public int Width { get; init; }
        public int Height { get; init; }
        public long FileSize { get; init; }
    }

    public record ImageVariantsResult
    {
        public ImageProcessingResult Thumbnail { get; init; } = null!;  // 150px
        public ImageProcessingResult Medium { get; init; } = null!;     // 600px
        public ImageProcessingResult Large { get; init; } = null!;      // 1200px
        public ImageProcessingResult WebP { get; init; } = null!;       // Original en WebP
    }
}
