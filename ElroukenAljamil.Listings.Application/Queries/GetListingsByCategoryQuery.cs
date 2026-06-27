using MediatR;
using ElroukenAljamil.Listings.Application.DTOs;


namespace ElroukenAljamil.Listings.Application.Queries
{
    public record GetListingsByCategoryQuery(Guid CategoryId, int Page = 1, int PageSize = 20)
      : IRequest<IReadOnlyList<ListingDto>>;

}
