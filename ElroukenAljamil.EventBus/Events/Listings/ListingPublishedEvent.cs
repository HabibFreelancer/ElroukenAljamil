using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.BuildingBlocks.EventBus.Events.Listings
{
    public record ListingPublishedEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
        public string SellerName { get; init; } = string.Empty;
        public Guid SellerId { get; init; }
        public List<string> ImageUrls { get; init; } = new();
        public string? City { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public string Currency { get; set; }
        public DateTime PublishedAt { get; set; }
    }

    public record ListingUpdatedEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
        public List<string> ImageUrls { get; init; } = new();
        public string? City { get; init; }
        public string Currency { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public record ListingDeactivatedEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public Guid SellerId { get; init; }
        public string Reason { get; init; } = string.Empty;
        public DateTime DeactivatedAt { get; set; }
    }

    public record ListingExpiringSoonEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public Guid SellerId { get; init; }
        public string Title { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }
    public record ListingCreatedIntegrationEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Category { get; init; } = string.Empty;
        public string SellerName { get; init; } = string.Empty;
        public Guid SellerId { get; init; }
        public List<string> ImageUrls { get; init; } = new();
        public string? City { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
    }
}
