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

        public Task<List<AnnonceAdType>> GetAllAsync(CancellationToken ct = default) =>
            _context.AdTypes
                .Include(a => a.Category)
                .OrderBy(a => a.CategoryId).ThenBy(a => a.DisplayOrder)
                .ToListAsync(ct);

        public Task<AnnonceAdType?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.AdTypes.Include(a => a.Category).FirstOrDefaultAsync(a => a.Id == id, ct);

        public Task<List<AnnonceAdType>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default) =>
            _context.AdTypes.Where(a => a.CategoryId == categoryId)
                .OrderBy(a => a.DisplayOrder).ToListAsync(ct);

        public async Task<List<AnnonceAdType>> GetByCategoryWithFallbackAsync(int categoryId, CancellationToken ct = default)
        {
            var adTypes = await _context.AdTypes
                .Where(a => a.CategoryId == categoryId && a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync(ct);

            if (adTypes.Count > 0) return adTypes;

            var parentId = await _context.Categories
                .Where(c => c.Id == categoryId)
                .Select(c => c.ParentCategoryId)
                .FirstOrDefaultAsync(ct);

            if (parentId == null) return adTypes;

            return await _context.AdTypes
                .Where(a => a.CategoryId == parentId && a.IsActive)
                .OrderBy(a => a.DisplayOrder)
                .ToListAsync(ct);
        }

        public async Task AddAsync(AnnonceAdType adType, CancellationToken ct = default)
        {
            await _context.AdTypes.AddAsync(adType, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(AnnonceAdType adType, CancellationToken ct = default)
        {
            _context.AdTypes.Update(adType);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(AnnonceAdType adType, CancellationToken ct = default)
        {
            _context.AdTypes.Remove(adType);
            await _context.SaveChangesAsync(ct);
        }
    }
}
