using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface IAdTypeRepository
    {
        Task<List<ListingAdType>> GetAllAsync(CancellationToken ct = default);
        Task<ListingAdType?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<ListingAdType>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default);
        Task AddAsync(ListingAdType adType, CancellationToken ct = default);
        Task UpdateAsync(ListingAdType adType, CancellationToken ct = default);
        Task DeleteAsync(ListingAdType adType, CancellationToken ct = default);
    }
}
