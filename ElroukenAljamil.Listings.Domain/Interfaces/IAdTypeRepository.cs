using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface IAdTypeRepository
    {
        Task<List<AnnonceAdType>> GetAllAsync(CancellationToken ct = default);
        Task<AnnonceAdType?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<List<AnnonceAdType>> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default);
        Task AddAsync(AnnonceAdType adType, CancellationToken ct = default);
        Task UpdateAsync(AnnonceAdType adType, CancellationToken ct = default);
        Task DeleteAsync(AnnonceAdType adType, CancellationToken ct = default);
    }
}
