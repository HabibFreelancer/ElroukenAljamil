using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Messaging.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Queries.GetConversationDetail
{
    public record GetConversationDetailQuery(Guid ConversationId) : IRequest<Result<ConversationDetailDto>>;
}
