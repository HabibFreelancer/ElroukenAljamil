using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces;

public interface IAnnonceRepository
{
    Task<IEnumerable<Annonce>> GetAllAsync();
    Task<Annonce?> GetByIdAsync(int id);
    Task<IEnumerable<Annonce>> GetByUserIdAsync(string userId, string? search, string? status, string? sortBy);
    Task<Annonce> AddAsync(Annonce annonce);
    Task UpdateAsync(Annonce annonce);
    Task DeleteAsync(int id);
    Task<IEnumerable<int>> GetCategoryIdsByKeywordAsync(string keyword);
    Task<IEnumerable<Annonce>> GetByCategoryForEstimateAsync(int categoryId, int take);
}
