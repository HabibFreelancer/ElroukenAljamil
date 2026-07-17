using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Search.Application.DTOs
{
    public record SearchRequestDto
    {
        public string? Query { get; init; }
        public string? Category { get; init; }
        public string? City { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public double? Latitude { get; init; }
        public double? Longitude { get; init; }
        public double? RadiusKm { get; init; }
        public Guid? SellerId { get; init; }
        public string SortBy { get; init; } = "relevance";
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }

    public record SearchResultDto
    {
        public List<SearchListingItemDto> Items { get; init; } = new();
        public long TotalCount { get; init; }
        public int Page { get; init; }
        public int PageSize { get; init; }
        public int TotalPages { get; init; }
        public bool HasNextPage { get; init; }
        public bool HasPreviousPage { get; init; }
        public double SearchDurationMs { get; init; }
    }

    public record SearchListingItemDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public decimal Price { get; init; }
        public string Currency { get; init; } = "EUR";
        public string Category { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string? ThumbnailUrl { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public DateTime PublishedAt { get; init; }
    }

    public record SuggestResponseDto
    {
        public List<string> Suggestions { get; init; } = new();
    }

    public record FacetsResponseDto
    {
        public List<CategoryFacetDto> Categories { get; init; } = new();
        public PriceRangeDto? PriceRange { get; init; }
    }

    public record CategoryFacetDto(string Name, long Count);

    public record PriceRangeDto(decimal Min, decimal Max, decimal Average);
}
