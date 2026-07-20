using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class MenuRepository : IMenuRepository
    {
        private readonly ListingsDbContext _context;

        public MenuRepository(ListingsDbContext context) => _context = context;

        public async Task<IReadOnlyList<ListingMenu>> GetAllAsync(CancellationToken ct = default) =>
            await _context.Menus
                .Where(m => m.IsActive)
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync(ct);

        public async Task<ListingMenu?> GetByIdAsync(int id, CancellationToken ct = default) =>
            await _context.Menus.FirstOrDefaultAsync(m => m.Id == id, ct);

        public async Task<ListingMenu> AddAsync(ListingMenu menu, CancellationToken ct = default)
        {
            await _context.Menus.AddAsync(menu, ct);
            await _context.SaveChangesAsync(ct);
            return menu;
        }

        public async Task UpdateAsync(ListingMenu menu, CancellationToken ct = default)
        {
            _context.Menus.Update(menu);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(ListingMenu menu, CancellationToken ct = default)
        {
            _context.Menus.Remove(menu);
            await _context.SaveChangesAsync(ct);
        }
    }
}
