using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface IMenuRepository
    {
        Task<IReadOnlyList<AnnonceMenu>> GetAllAsync(CancellationToken ct = default);
        Task<AnnonceMenu?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<AnnonceMenu> AddAsync(AnnonceMenu menu, CancellationToken ct = default);
        Task UpdateAsync(AnnonceMenu menu, CancellationToken ct = default);
        Task DeleteAsync(AnnonceMenu menu, CancellationToken ct = default);
    }
}
