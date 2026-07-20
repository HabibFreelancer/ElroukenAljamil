using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IReadOnlyList<ListingCategory>> GetAllAsync(CancellationToken ct = default);
        Task<ListingCategory?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<ListingCategory>> GetByMenuIdAsync(int menuId, CancellationToken ct = default);
        Task<IReadOnlyList<ListingCategory>> GetForDepositAsync(int menuId, CancellationToken ct = default);
        Task<IReadOnlyList<ListingCategory>> GetTreeAsync(int menuId, CancellationToken ct = default);
        Task<ListingCategory> AddAsync(ListingCategory category, CancellationToken ct = default);
        Task UpdateAsync(ListingCategory category, CancellationToken ct = default);
        Task DeleteAsync(ListingCategory category, CancellationToken ct = default);
        Task<bool> ExistsAsync(int id, CancellationToken ct = default);
    }
}
