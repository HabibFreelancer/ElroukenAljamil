using ElroukenAljamil.Messaging.Domain.Entities;

namespace ElroukenAljamil.Messaging.Domain.Interfaces
{
    public interface IConversationRepository
    {
        Task<Conversation?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
        Task AddAsync(Conversation conversation, CancellationToken cancellationToken = default);
        Task UpdateAsync(Conversation conversation, CancellationToken cancellationToken = default);
    }

}
