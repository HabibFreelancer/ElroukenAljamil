

using MediatR;
using ElroukenAljamil.Listings.Application.DTOs;

namespace ElroukenAljamil.Listings.Application.Queries
{
    public record GetListingByIdQuery(Guid ListingId) : IRequest<ListingDto?>;

}
