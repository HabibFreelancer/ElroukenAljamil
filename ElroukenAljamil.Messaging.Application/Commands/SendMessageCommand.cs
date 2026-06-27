using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands
{
    public record SendMessageCommand(Guid ConversationId, Guid SenderId, string Content) : IRequest<Guid>;


    public record StartConversationCommand(Guid ListingId, Guid BuyerId, Guid SellerId, string InitialMessage) : IRequest<Guid>;


}
