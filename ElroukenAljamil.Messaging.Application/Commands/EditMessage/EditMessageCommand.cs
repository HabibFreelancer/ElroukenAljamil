using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.EditMessage
{
    public record EditMessageCommand : IRequest<Result>
    {
        public Guid ConversationId { get; init; }
        public Guid MessageId { get; init; }
        public string NewContent { get; init; } = string.Empty;
    }
}
