using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Application.DTOs;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Queries.GetConversations
{
    public class GetConversationsQueryHandler : IRequestHandler<GetConversationsQuery, Result<ConversationListDto>>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public GetConversationsQueryHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result<ConversationListDto>> Handle(GetConversationsQuery request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<ConversationListDto>.Failure("Utilisateur non authentifié.");

            var (conversations, totalCount) = await _repository.GetPagedByUserAsync(
                _currentUser.UserId, request.Page, request.PageSize, ct);

            var totalUnread = await _repository.GetUnreadCountByUserAsync(_currentUser.UserId, ct);

            var dtos = conversations.Select(c => new ConversationDto
            {
                Id = c.Id,
                BuyerId = c.BuyerId,
                BuyerName = c.BuyerName,
                SellerId = c.SellerId,
                SellerName = c.SellerName,
                ListingId = c.ListingId,
                ListingTitle = c.ListingTitle,
                Status = c.Status.ToString(),
                UnreadCount = c.GetUnreadCount(_currentUser.UserId),
                LastMessage = c.LastMessage is not null ? new MessageDto
                {
                    Id = c.LastMessage.Id,
                    SenderId = c.LastMessage.SenderId,
                    SenderName = c.LastMessage.SenderName,
                    Content = c.LastMessage.Content.Length > 100
                        ? c.LastMessage.Content[..100] + "..."
                        : c.LastMessage.Content,
                    SentAt = c.LastMessage.SentAt,
                    IsRead = c.LastMessage.IsRead,
                    IsMine = c.LastMessage.SenderId == _currentUser.UserId
                } : null,
                LastMessageAt = c.LastMessageAt,
                CreatedAt = c.CreatedAt
            }).ToList();

            return Result<ConversationListDto>.Success(new ConversationListDto
            {
                Conversations = dtos,
                TotalCount = totalCount,
                Page = request.Page,
                PageSize = request.PageSize,
                TotalUnread = totalUnread
            });
        }
    }

}
