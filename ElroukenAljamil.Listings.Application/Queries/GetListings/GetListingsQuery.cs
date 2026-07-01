using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetListings
{
    public record GetListingsQuery : IRequest<Result<BuildingBlocks.Common.Results.PagedResult<ListingSummaryDto>>>
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
        public string? Category { get; init; }
        public string? Status { get; init; }
        public Guid? SellerId { get; init; }
    }
}
