using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.DeleteMessage
{
    public record DeleteMessageCommand : IRequest<Result>
    {
        public Guid ConversationId { get; init; }
        public Guid MessageId { get; init; }
    }
}
