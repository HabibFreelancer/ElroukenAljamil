using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Messaging.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.SendMessage
{
    public record SendMessageCommand : IRequest<Result<MessageDto>>
    {
        public Guid ConversationId { get; init; }
        public string Content { get; init; } = string.Empty;
    }

}
