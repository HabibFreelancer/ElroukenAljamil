using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.StartConversation
{
    public record StartConversationCommand : IRequest<Result<Guid>>
    {
        public Guid SellerId { get; init; }
        public string SellerName { get; init; } = string.Empty;
        public Guid ListingId { get; init; }
        public string ListingTitle { get; init; } = string.Empty;
        public string Message { get; init; } = string.Empty;
    }
}
