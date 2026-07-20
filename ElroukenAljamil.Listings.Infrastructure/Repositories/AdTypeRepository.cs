using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class AdTypeRepository : IAdTypeRepository
    {
        private readonly ListingsDbContext _context;
        public AdTypeRepository(ListingsDbContext context) => _context = context;

        public Task<List<ListingAdType>> GetAllAsync(CancellationToken ct = default) =>
            _context.AdTypes
                .Include(a => a.Category)
                .OrderBy(a => a.CategoryId).ThenBy(a => a.DisplayOrder)
                .ToListAsync(ct);

        public Task<ListingAdType?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.AdTypes.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id, ct);

        public Task<List<ListingAdType>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default) =>
            _context.AdTypes.Where(a => a.CategoryId == categoryId)
                .OrderBy(a => a.DisplayOrder).ToListAsync(ct);

        public async Task AddAsync(ListingAdType adType, CancellationToken ct = default)
        {
            await _context.AdTypes.AddAsync(adType, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ListingAdType adType, CancellationToken ct = default)
        {
            _context.AdTypes.Update(adType);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(ListingAdType adType, CancellationToken ct = default)
        {
            _context.AdTypes.Remove(adType);
            await _context.SaveChangesAsync(ct);
        }
    }
}
