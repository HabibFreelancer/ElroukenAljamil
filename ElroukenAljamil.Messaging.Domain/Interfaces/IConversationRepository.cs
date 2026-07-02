using ElroukenAljamil.Messaging.Domain.Entities;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
namespace ElroukenAljamil.Messaging.Domain.Interfaces
{
    public interface IConversationRepository : IRepository<Conversation>
    {
        Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken ct = default);
        Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
        Task<Conversation?> GetExistingConversationAsync(
            Guid buyerId, Guid sellerId, Guid listingId, CancellationToken ct = default);
        Task<int> GetUnreadCountByUserAsync(Guid userId, CancellationToken ct = default);
        Task<(IReadOnlyList<Conversation> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId, int page, int pageSize, CancellationToken ct = default);
    }

}
