using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeactivateListing
{
    public record DeactivateListingCommand(Guid Id) : IRequest<Result>;
}
