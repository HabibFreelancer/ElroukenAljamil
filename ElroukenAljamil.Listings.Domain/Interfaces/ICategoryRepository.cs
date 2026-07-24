using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<AnnonceCategory>> GetAllAsync(CancellationToken ct = default);
        Task<AnnonceCategory?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<AnnonceCategory>> GetByMenuIdAsync(int menuId, CancellationToken ct = default);
        Task<IReadOnlyList<AnnonceCategory>> GetForDepositAsync(int menuId, CancellationToken ct = default);
        Task<IReadOnlyList<AnnonceCategory>> GetTreeAsync(int menuId, CancellationToken ct = default);
        Task<AnnonceCategory> AddAsync(AnnonceCategory category, CancellationToken ct = default);
        Task UpdateAsync(AnnonceCategory category, CancellationToken ct = default);
        Task DeleteAsync(AnnonceCategory category, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
