using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.SearchListings
{
    public record SearchListingsQuery : IRequest<Result<SearchResultDto>>
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
}
