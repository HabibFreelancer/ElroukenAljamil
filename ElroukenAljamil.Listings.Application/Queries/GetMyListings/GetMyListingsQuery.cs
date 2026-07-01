using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetMyListings
{
    public record GetMyListingsQuery : IRequest<Result<List<ListingSummaryDto>>>;

}
