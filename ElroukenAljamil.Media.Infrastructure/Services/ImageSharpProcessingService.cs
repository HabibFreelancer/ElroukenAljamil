using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

namespace ElroukenAljamil.Media.Infrastructure.Services
{
    /// <summary>
    /// Service de traitement d'images via SixLabors.ImageSharp.
    /// Génère les variantes (thumbnail, medium, large) et convertit en WebP.
    /// </summary>
    public class ImageSharpProcessingService : IImageProcessingService
    {
        private readonly ILogger<ImageSharpProcessingService> _logger;

        public ImageSharpProcessingService(ILogger<ImageSharpProcessingService> logger)
        {
            _logger = logger;
        }

        public async Task<ImageProcessingResult> ResizeAsync(
            Stream inputStream, int maxWidth,
            string outputFormat = "webp", int quality = 80,
            CancellationToken ct = default)
        {
            inputStream.Position = 0;
            using var image = await Image.LoadAsync(inputStream, ct);

            // Ne redimensionner que si l'image est plus large que maxWidth
            if (image.Width > maxWidth)
            {
                var ratio = (double)maxWidth / image.Width;
                var newHeight = (int)(image.Height * ratio);
                image.Mutate(x => x.Resize(maxWidth, newHeight));
            }

            var outputStream = new MemoryStream();
            var encoder = new WebpEncoder { Quality = quality };
            await image.SaveAsync(outputStream, encoder, ct);
            outputStream.Position = 0;

            return new ImageProcessingResult
            {
                OutputStream = outputStream,
                ContentType = "image/webp",
                Width = image.Width,
                Height = image.Height,
                FileSize = outputStream.Length
            };
        }

        public async Task<ImageVariantsResult> GenerateVariantsAsync(
            Stream inputStream, CancellationToken ct = default)
        {
            _logger.LogInformation("Génération des variantes d'image...");

            var thumbnail = await ResizeAsync(inputStream, maxWidth: 150, quality: 70, ct: ct);
            inputStream.Position = 0;

            var medium = await ResizeAsync(inputStream, maxWidth: 600, quality: 80, ct: ct);
            inputStream.Position = 0;

            var large = await ResizeAsync(inputStream, maxWidth: 1200, quality: 85, ct: ct);
            inputStream.Position = 0;

            var webp = await ResizeAsync(inputStream, maxWidth: 2000, quality: 80, ct: ct);

            _logger.LogInformation(
                "Variantes générées - Thumbnail: {T}KB, Medium: {M}KB, Large: {L}KB, WebP: {W}KB",
                thumbnail.FileSize / 1024,
                medium.FileSize / 1024,
                large.FileSize / 1024,
                webp.FileSize / 1024);

            return new ImageVariantsResult
            {
                Thumbnail = thumbnail,
                Medium = medium,
                Large = large,
                WebP = webp
            };
        }

        public async Task<ImageDimensions> GetDimensionsAsync(Stream inputStream, CancellationToken ct = default)
        {
            inputStream.Position = 0;
            var imageInfo = await Image.IdentifyAsync(inputStream, ct);

            if (imageInfo is null)
                throw new InvalidOperationException("Impossible de lire les dimensions de l'image.");

            return new ImageDimensions(imageInfo.Width, imageInfo.Height);
        }

        public async Task<bool> IsValidImageAsync(Stream inputStream, CancellationToken ct = default)
        {
            try
            {
                inputStream.Position = 0;
                var imageInfo = await Image.IdentifyAsync(inputStream, ct);
                return imageInfo is not null;
            }
            catch
            {
                return false;
            }
        }
    }
}
