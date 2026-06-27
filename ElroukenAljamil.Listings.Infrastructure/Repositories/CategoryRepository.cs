using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ListingsDbContext _context;


        public CategoryRepository(ListingsDbContext context)
        {
            _context = context;
        }


        public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _context.Categories.FindAsync(new object[] { id }, cancellationToken);
        }


        public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);
        }


        public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
        {
            await _context.Categories.AddAsync(category, cancellationToken);
        }
    }

}
