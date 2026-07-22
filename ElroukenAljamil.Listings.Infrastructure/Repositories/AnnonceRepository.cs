using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories;

public class AnnonceRepository : IAnnonceRepository
{
    private readonly ListingsDbContext _context;
    public AnnonceRepository(ListingsDbContext context) => _context = context;

    public async Task<IEnumerable<Annonce>> GetAllAsync() =>
        await _context.Annonces.OrderByDescending(a => a.CreatedAt).ToListAsync();

    public async Task<Annonce?> GetByIdAsync(int id) =>
        await _context.Annonces.Include(a => a.Category).ThenInclude(c => c!.Menu)
            .FirstOrDefaultAsync(a => a.Id == id);

    public async Task<IEnumerable<Annonce>> GetByUserIdAsync(string userId, string? search, string? status, string? sortBy)
    {
        var query = _context.Annonces.AsQueryable();

        if (!string.IsNullOrEmpty(userId))
            query = query.Where(a => a.UserId == userId);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Title.Contains(search) || a.Description.Contains(search));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status == status);
        else
            query = query.Where(a => a.Status != "draft");

        query = sortBy switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            _ => query.OrderByDescending(a => a.CreatedAt)
        };

        return await query.ToListAsync();
    }

    public async Task<Annonce> AddAsync(Annonce annonce)
    {
        _context.Annonces.Add(annonce);
        await _context.SaveChangesAsync();
        return annonce;
    }

    public async Task UpdateAsync(Annonce annonce)
    {
        _context.Annonces.Update(annonce);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var annonce = await _context.Annonces.FindAsync(id);
        if (annonce != null)
        {
            _context.Annonces.Remove(annonce);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<int>> GetCategoryIdsByKeywordAsync(string keyword) =>
        await _context.Annonces
            .Where(a => a.Title.Contains(keyword) || a.Description.Contains(keyword))
            .Select(a => a.CategoryId)
            .Distinct()
            .ToListAsync();

    public async Task<IEnumerable<Annonce>> GetByCategoryForEstimateAsync(int categoryId, int take) =>
        await _context.Annonces
            .Where(a => a.CategoryId == categoryId && a.Price > 0)
            .OrderByDescending(a => a.CreatedAt)
            .Take(take)
            .ToListAsync();
}
