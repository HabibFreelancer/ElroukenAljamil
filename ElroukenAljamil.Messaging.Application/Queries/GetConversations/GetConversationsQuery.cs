using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Messaging.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Queries.GetConversations
{
    public record GetConversationsQuery : IRequest<Result<ConversationListDto>>
    {
        public int Page { get; init; } = 1;
        public int PageSize { get; init; } = 20;
    }
}
