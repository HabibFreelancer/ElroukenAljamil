

using MediatR;


namespace ElroukenAljamil.Listings.Application.Commands
{
    public record PublishListingCommand(Guid ListingId) : IRequest<Unit>;

}
