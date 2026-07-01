using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Listings.Application.DTOs
{
    public record ListingDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public string Category { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public List<string> ImageUrls { get; init; } = new();
        public string Status { get; init; } = string.Empty;
        public Guid SellerId { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public int ViewCount { get; init; }
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
        public DateTime? ExpiresAt { get; init; }
    }
    public record ListingSummaryDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public string Category { get; init; } = string.Empty;
        public string Location { get; init; } = string.Empty;
        public string? ThumbnailUrl { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }

    public record PagedResult<T>
    {
        public List<T> Items { get; init; } = new();
        public int TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        public bool HasNextPage => Page < TotalPages;
        public bool HasPreviousPage => Page > 1;
    }

}
