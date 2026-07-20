using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<IReadOnlyList<ListingMenu>> GetAllAsync(CancellationToken ct = default);
        Task<ListingMenu?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<ListingMenu> AddAsync(ListingMenu menu, CancellationToken ct = default);
        Task UpdateAsync(ListingMenu menu, CancellationToken ct = default);
        Task DeleteAsync(ListingMenu menu, CancellationToken ct = default);
    }
}
