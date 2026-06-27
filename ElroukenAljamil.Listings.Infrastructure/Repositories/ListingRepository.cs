using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    /// <summary>
    /// Implémentation concrète du repository Listings avec EF Core.
    /// </summary>
    public class ListingRepository : IListingRepository
    {
        private readonly ListingsDbContext _context;


        public ListingRepository(ListingsDbContext context)
        {
            _context = context;
        }


        public async Task<Listing?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Listings
                .Include(l => l.Images)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }


        public async Task<IReadOnlyList<Listing>> GetBySellerIdAsync(Guid sellerId, CancellationToken cancellationToken = default)
        {
            return await _context.Listings
                .Include(l => l.Images)
                .Where(l => l.SellerId == sellerId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync(cancellationToken);
        }


        public async Task<IReadOnlyList<Listing>> GetActiveByCategoryAsync(
            Guid categoryId, int page, int pageSize, CancellationToken cancellationToken = default)
        {
            return await _context.Listings
                .Include(l => l.Images)
                .Where(l => l.CategoryId == categoryId && l.Status == ListingStatus.Active)
                .OrderByDescending(l => l.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
        }


        public async Task AddAsync(Listing listing, CancellationToken cancellationToken = default)
        {
            await _context.Listings.AddAsync(listing, cancellationToken);
        }


        public Task UpdateAsync(Listing listing, CancellationToken cancellationToken = default)
        {
            _context.Listings.Update(listing);
            return Task.CompletedTask;
        }


        public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var listing = await _context.Listings.FindAsync(new object[] { id }, cancellationToken);
            if (listing is not null)
                _context.Listings.Remove(listing);
        }
    }



}
