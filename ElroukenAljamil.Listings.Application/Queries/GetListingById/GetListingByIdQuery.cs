

using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetListingById
{
    public record GetListingByIdQuery(Guid ListingId) : IRequest<Result<ListingDto>>;

}
