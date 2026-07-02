using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Messaging.Application.Commands.DeleteMessage
{
    public class DeleteMessageCommandHandler : IRequestHandler<DeleteMessageCommand, Result>
    {
        private readonly IConversationRepository _repository;
        private readonly ICurrentUserService _currentUser;

        public DeleteMessageCommandHandler(
            IConversationRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(DeleteMessageCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result.Failure("Utilisateur non authentifié.");

            var conversation = await _repository.GetByIdWithMessagesAsync(request.ConversationId, ct);
            if (conversation is null)
                return Result.Failure($"Conversation {request.ConversationId} introuvable.");

            if (!conversation.IsParticipant(_currentUser.UserId))
                return Result.Failure("Vous n'êtes pas participant à cette conversation.");

            var message = conversation.Messages.FirstOrDefault(m => m.Id == request.MessageId);
            if (message is null)
                return Result.Failure($"Message {request.MessageId} introuvable.");

            try
            {
                message.Delete(_currentUser.UserId);
            }
            catch (InvalidOperationException ex)
            {
                return Result.Failure(ex.Message);
            }

            await _repository.UpdateAsync(conversation, ct);
            return Result.Success();
        }
    }
}
