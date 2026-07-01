using System.Linq.Expressions;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Listings.Application.Interfaces;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;



namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class ListingRepository : IListingRepository
    {
        private readonly ListingsDbContext _context;

        public ListingRepository(ListingsDbContext context)
        {
            _context = context;
        }

        public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Listings.FirstOrDefaultAsync(l => l.Id == id, ct);
        }

        public async Task<IReadOnlyList<Listing>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Listings.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId, CancellationToken ct = default)
        {
            return await _context.Listings
                .Where(l => l.SellerId == sellerId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Listing>> GetByCategoryAsync(string category, CancellationToken ct = default)
        {
            return await _context.Listings
                .Where(l => l.Category.Name == category && l.Status == ListingStatus.Active)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<Listing>> GetByStatusAsync(ListingStatus status, CancellationToken ct = default)
        {
            return await _context.Listings
                .Where(l => l.Status == status)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<(IReadOnlyList<Listing> Items, int TotalCount)> GetPagedAsync(
            int page, int pageSize,
            ListingStatus? status = null,
            string? category = null,
            Guid? sellerId = null,
            CancellationToken ct = default)
        {
            var query = _context.Listings.AsQueryable();

            // Filtres
            if (status.HasValue)
                query = query.Where(l => l.Status == status.Value);
            else
                query = query.Where(l => l.Status == ListingStatus.Active); // Par défaut : actives

            if (!string.IsNullOrEmpty(category))
                query = query.Where(l => l.Category.Name == category);

            if (sellerId.HasValue)
                query = query.Where(l => l.SellerId == sellerId.Value);

            // Compter le total
            var totalCount = await query.CountAsync(ct);

            // Pagination
            var items = await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return (items, totalCount);
        }

        public async Task<int> GetCountBySellerAsync(Guid sellerId, CancellationToken ct = default)
        {
            return await _context.Listings
                .CountAsync(l => l.SellerId == sellerId, ct);
        }

        public async Task<IReadOnlyList<Listing>> GetExpiredListingsAsync(CancellationToken ct = default)
        {
            return await _context.Listings
                .Where(l => l.Status == ListingStatus.Active &&
                            l.ExpiresAt.HasValue &&
                            l.ExpiresAt.Value < DateTime.UtcNow)
                .ToListAsync(ct);
        }

        public async Task AddAsync(Listing entity, CancellationToken ct = default)
        {
            await _context.Listings.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Listing entity, CancellationToken ct = default)
        {
            _context.Listings.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Listing entity, CancellationToken ct = default)
        {
            _context.Listings.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }
                     
        public Task<IReadOnlyList<Listing>> FindAsync(Expression<Func<Listing, bool>> predicate, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        Task<Listing> IRepository<Listing>.AddAsync(Listing entity, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public Task<int> CountAsync(Expression<Func<Listing, bool>>? predicate = null, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }




}
