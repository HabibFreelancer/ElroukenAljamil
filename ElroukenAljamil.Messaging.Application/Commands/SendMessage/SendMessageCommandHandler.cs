using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Messaging;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Application.DTOs;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.SendMessage
{
    public class SendMessageCommandHandler : IRequestHandler<SendMessageCommand, Result<MessageDto>>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public SendMessageCommandHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<MessageDto>.Failure("Utilisateur non authentifié.");

            var conversation = await _repository.GetByIdWithMessagesAsync(request.ConversationId, ct);
            if (conversation is null)
                return Result<MessageDto>.Failure($"Conversation {request.ConversationId} introuvable.");

            if (!conversation.IsParticipant(_currentUser.UserId))
                return Result<MessageDto>.Failure("Vous n'êtes pas participant à cette conversation.");

            var message = conversation.SendMessage(
                _currentUser.UserId,
                _currentUser.FullName ?? "Utilisateur",
                request.Content);

            await _repository.UpdateAsync(conversation, ct);

            // Publier l'événement pour les notifications
            var recipientId = conversation.GetRecipientId(_currentUser.UserId);
            await _eventBus.PublishAsync(new NewMessageReceivedEvent
            {
                ConversationId = conversation.Id,
                SenderId = _currentUser.UserId,
                SenderName = _currentUser.FullName ?? "Utilisateur",
                RecipientId = recipientId,
                MessagePreview = request.Content.Length > 100 ? request.Content[..100] + "..." : request.Content,
                ListingId = conversation.ListingId,
                ListingTitle = conversation.ListingTitle,
                SentAt = DateTime.UtcNow
            }, ct);

            var dto = new MessageDto
            {
                Id = message.Id,
                SenderId = message.SenderId,
                SenderName = message.SenderName,
                Content = message.Content,
                SentAt = message.SentAt,
                IsRead = message.IsRead,
                ReadAt = message.ReadAt,
                IsEdited = message.IsEdited,
                IsDeleted = message.IsDeleted,
                IsMine = true
            };

            return Result<MessageDto>.Success(dto);
        }
    }

}
