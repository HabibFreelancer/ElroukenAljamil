using ElroukenAljamil.BuildingBlocks.Common;
using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Search.Domain.Entities
{
    /// <summary>
    /// Document indexé dans Elasticsearch représentant une annonce recherchable.
    /// Ce n'est pas un agrégat au sens DDD classique — c'est une projection
    /// dénormalisée optimisée pour la recherche.
    /// </summary>
    public class SearchableListingDocument : BaseEntity
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Currency { get; set; } = "EUR";
        public string Category { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public Guid SellerId { get; set; }
        public string SellerName { get; set; } = string.Empty;
        public List<string> ImageUrls { get; set; } = new();
        public string? ThumbnailUrl { get; set; }
        public string Status { get; set; } = "Active";
        public DateTime PublishedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Champ calculé pour la recherche full-text (titre + description concaténés).
        /// </summary>
        public string SearchText => $"{Title} {Description} {Category} {City}";

        /// <summary>
        /// Coordonnées pour la recherche géographique (format Elasticsearch GeoPoint).
        /// </summary>
        public GeoLocation? Location => Latitude.HasValue && Longitude.HasValue
            ? new GeoLocation(Latitude.Value, Longitude.Value)
            : null;
    }

    /// <summary>
    /// Value Object pour les coordonnées géographiques (compatible Elasticsearch).
    /// </summary>
    public record GeoLocation(double Lat, double Lon);
}
