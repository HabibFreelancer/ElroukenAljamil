using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.MarkAsRead
{
    public class MarkAsReadCommandHandler : IRequestHandler<MarkAsReadCommand, Result>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public MarkAsReadCommandHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(MarkAsReadCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result.Failure("Utilisateur non authentifié.");

            var conversation = await _repository.GetByIdWithMessagesAsync(request.ConversationId, ct);
            if (conversation is null)
                return Result.Failure($"Conversation {request.ConversationId} introuvable.");

            if (!conversation.IsParticipant(_currentUser.UserId))
                return Result.Failure("Vous n'êtes pas participant à cette conversation.");

            conversation.MarkAsRead(_currentUser.UserId);
            await _repository.UpdateAsync(conversation, ct);

            return Result.Success();
        }
    }

}
