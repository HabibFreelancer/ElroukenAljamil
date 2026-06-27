using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.EventBus.Events.Listings
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
    }

    public record ListingDeactivatedEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public Guid SellerId { get; init; }
        public string Reason { get; init; } = string.Empty;
    }

    public record ListingExpiringSoonEvent : IntegrationEvent
    {
        public Guid ListingId { get; init; }
        public Guid SellerId { get; init; }
        public string Title { get; init; } = string.Empty;
        public DateTime ExpiresAt { get; init; }
    }

}
