using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ListingsDbContext _context;

        public CategoryRepository(ListingsDbContext context) => _context = context;

        public async Task<IReadOnlyList<ListingCategory>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Categories
                .Include(c => c.Menu)
                .OrderBy(c => c.MenuId).ThenBy(c => c.DisplayOrder)
                .ToListAsync(ct);

        public async Task<ListingCategory?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Categories
                .Include(c => c.Menu)
                .FirstOrDefaultAsync(c => c.Id == id, ct);

        public async Task<IReadOnlyList<ListingCategory>> GetByMenuIdAsync(int menuId, CancellationToken ct = default) =>
            await _context.Categories
                .Where(c => c.MenuId == menuId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<ListingCategory>> GetForDepositAsync(int menuId, CancellationToken ct = default) =>
            await _context.Categories
                .Where(c => c.MenuId == menuId && c.IsActive && c.ShowInDeposit)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(ct);

        public async Task<IReadOnlyList<ListingCategory>> GetTreeAsync(int menuId, CancellationToken ct = default)
        {
            // Charger toutes les catégories du menu pour que EF peuple les SubCategories
            await _context.Categories
                .Where(c => c.MenuId == menuId && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .LoadAsync(ct);

            return await _context.Categories
                .Where(c => c.MenuId == menuId && c.IsActive && c.ParentCategoryId == null)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync(ct);
        }

        public async Task<ListingCategory> AddAsync(ListingCategory category, CancellationToken ct = default)
        {
            await _context.Categories.AddAsync(category, ct);
            await _context.SaveChangesAsync(ct);
            return category;
        }

        public async Task UpdateAsync(ListingCategory category, CancellationToken ct = default)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(ListingCategory category, CancellationToken ct = default)
        {
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(int id, CancellationToken ct = default) =>
            await _context.Categories.AnyAsync(c => c.Id == id, ct);
    }
}
