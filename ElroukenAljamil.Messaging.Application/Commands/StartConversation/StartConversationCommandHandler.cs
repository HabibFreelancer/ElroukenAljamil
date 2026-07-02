using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Messaging;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Domain.Entities;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.StartConversation
{
    public class StartConversationCommandHandler : IRequestHandler<StartConversationCommand, Result<Guid>>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public StartConversationCommandHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result<Guid>> Handle(StartConversationCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<Guid>.Failure("Utilisateur non authentifié.");

            if (_currentUser.UserId == request.SellerId)
                return Result<Guid>.Failure("Vous ne pouvez pas vous contacter vous-même.");

            // Vérifier si une conversation existe déjà pour ce triplet (buyer, seller, listing)
            var existing = await _repository.GetExistingConversationAsync(
                _currentUser.UserId, request.SellerId, request.ListingId, ct);

            if (existing is not null)
            {
                // Ajouter le message à la conversation existante
                existing.SendMessage(_currentUser.UserId, _currentUser.FullName ?? "Utilisateur", request.Message);
                await _repository.UpdateAsync(existing, ct);

                await PublishNewMessageEvent(existing, _currentUser.UserId, request.Message, ct);
                return Result<Guid>.Success(existing.Id);
            }

            // Créer une nouvelle conversation
            var conversation = Conversation.Create(
                buyerId: _currentUser.UserId,
                buyerName: _currentUser.FullName ?? "Utilisateur",
                sellerId: request.SellerId,
                sellerName: request.SellerName,
                listingId: request.ListingId,
                listingTitle: request.ListingTitle,
                initialMessage: request.Message);

            await _repository.AddAsync(conversation, ct);

            await PublishNewMessageEvent(conversation, _currentUser.UserId, request.Message, ct);

            return Result<Guid>.Success(conversation.Id);
        }

        private async Task PublishNewMessageEvent(
            Conversation conversation, Guid senderId, string content, CancellationToken ct)
        {
            var recipientId = conversation.GetRecipientId(senderId);
            var senderName = senderId == conversation.BuyerId
                ? conversation.BuyerName
                : conversation.SellerName;

            await _eventBus.PublishAsync(new NewMessageReceivedEvent
            {
                ConversationId = conversation.Id,
                SenderId = senderId,
                SenderName = senderName,
                RecipientId = recipientId,
                MessagePreview = content.Length > 100 ? content[..100] + "..." : content,
                ListingId = conversation.ListingId,
                ListingTitle = conversation.ListingTitle,
                SentAt = DateTime.UtcNow
            }, ct);
        }
    }
}
