using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Media.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Media.Application.Queries.GetMediaByListing
{
    public record GetMediaByListingQuery(Guid ListingId) : IRequest<Result<List<MediaFileDto>>>;
}
