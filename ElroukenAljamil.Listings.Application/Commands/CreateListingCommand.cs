using MediatR;
using ElroukenAljamil.Listings.Application.DTOs;


namespace ElroukenAljamil.Listings.Application.Commands
{
    /// <summary>
    /// Command CQRS pour créer une nouvelle annonce.
    /// </summary>
    public record CreateListingCommand(
        string Title,
        string Description,
        decimal Price,
        string Currency,
        Guid SellerId,
        Guid CategoryId,
        string City,
        string PostalCode,
        string Country
    ) : IRequest<ListingDto>;

}
