using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Application.DTOs;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Queries.GetConversationDetail
{
    public class GetConversationDetailQueryHandler
    : IRequestHandler<GetConversationDetailQuery, Result<ConversationDetailDto>>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public GetConversationDetailQueryHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result<ConversationDetailDto>> Handle(
            GetConversationDetailQuery request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<ConversationDetailDto>.Failure("Utilisateur non authentifié.");

            var conversation = await _repository.GetByIdWithMessagesAsync(request.ConversationId, ct);
            if (conversation is null)
                return Result<ConversationDetailDto>.Failure(
                    $"Conversation {request.ConversationId} introuvable.");

            if (!conversation.IsParticipant(_currentUser.UserId))
                return Result<ConversationDetailDto>.Failure(
                    "Vous n'êtes pas participant à cette conversation.");

            var dto = new ConversationDetailDto
            {
                Id = conversation.Id,
                BuyerId = conversation.BuyerId,
                BuyerName = conversation.BuyerName,
                SellerId = conversation.SellerId,
                SellerName = conversation.SellerName,
                ListingId = conversation.ListingId,
                ListingTitle = conversation.ListingTitle,
                Status = conversation.Status.ToString(),
                Messages = conversation.Messages
                    .OrderBy(m => m.SentAt)
                    .Select(m => new MessageDto
                    {
                        Id = m.Id,
                        SenderId = m.SenderId,
                        SenderName = m.SenderName,
                        Content = m.Content,
                        SentAt = m.SentAt,
                        IsRead = m.IsRead,
                        ReadAt = m.ReadAt,
                        IsEdited = m.IsEdited,
                        IsDeleted = m.IsDeleted,
                        IsMine = m.SenderId == _currentUser.UserId
                    })
                    .ToList(),
                CreatedAt = conversation.CreatedAt
            };

            return Result<ConversationDetailDto>.Success(dto);
        }
    }
}
