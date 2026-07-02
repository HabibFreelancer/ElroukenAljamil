using System.Linq.Expressions;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Messaging.Domain.Entities;
using ElroukenAljamil.Messaging.Domain.Enums;
using ElroukenAljamil.Messaging.Domain.Interfaces;
using ElroukenAljamil.Messaging.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Messaging.Infrastructure.Repositories
{
    public class ConversationRepository : IConversationRepository
    {
        private readonly MessagingDbContext _context;

        public ConversationRepository(MessagingDbContext context)
        {
            _context = context;
        }

        public async Task<Conversation?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<Conversation?> GetByIdWithMessagesAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.Id == id, ct);
        }

        public async Task<IReadOnlyList<Conversation>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => (c.BuyerId == userId || c.SellerId == userId) &&
                            c.Status == ConversationStatus.Active)
                .OrderByDescending(c => c.LastMessageAt)
                .ToListAsync(ct);
        }

        public async Task<Conversation?> GetExistingConversationAsync(
            Guid buyerId, Guid sellerId, Guid listingId, CancellationToken ct = default)
        {
            return await _context.Conversations
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c =>
                    c.BuyerId == buyerId &&
                    c.SellerId == sellerId &&
                    c.ListingId == listingId &&
                    c.Status == ConversationStatus.Active, ct);
        }

        public async Task<int> GetUnreadCountByUserAsync(Guid userId, CancellationToken ct = default)
        {
            // Somme des messages non lus dans toutes les conversations actives
            var asBuyer = await _context.Conversations
                .Where(c => c.BuyerId == userId && c.Status == ConversationStatus.Active)
                .SumAsync(c => c.UnreadCountBuyer, ct);

            var asSeller = await _context.Conversations
                .Where(c => c.SellerId == userId && c.Status == ConversationStatus.Active)
                .SumAsync(c => c.UnreadCountSeller, ct);

            return asBuyer + asSeller;
        }

        public async Task<(IReadOnlyList<Conversation> Items, int TotalCount)> GetPagedByUserAsync(
            Guid userId, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Conversations
                .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(1))
                .Where(c => (c.BuyerId == userId || c.SellerId == userId) &&
                            c.Status == ConversationStatus.Active)
                .OrderByDescending(c => c.LastMessageAt);

            var totalCount = await query.CountAsync(ct);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<IReadOnlyList<Conversation>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Conversations.ToListAsync(ct);
        }

        public async Task AddAsync(Conversation entity, CancellationToken ct = default)
        {
            await _context.Conversations.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Conversation entity, CancellationToken ct = default)
        {
            _context.Conversations.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Conversation entity, CancellationToken ct = default)
        {
            _context.Conversations.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        Task<IReadOnlyList<Conversation>> IRepository<Conversation>.FindAsync(Expression<Func<Conversation, bool>> predicate, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<Conversation> IRepository<Conversation>.AddAsync(Conversation entity, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<Conversation>.ExistsAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<int> IRepository<Conversation>.CountAsync(Expression<Func<Conversation, bool>>? predicate, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
