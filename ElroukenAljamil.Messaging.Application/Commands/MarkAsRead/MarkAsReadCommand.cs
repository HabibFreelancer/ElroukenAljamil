using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.MarkAsRead
{
    public record MarkAsReadCommand(Guid ConversationId) : IRequest<Result>;
}
