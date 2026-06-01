using ElroukenAljamil.Domain.Entities;

namespace ElroukenAljamil.Domain.Interfaces;

public interface IAnnonceRepository
{
    Task<IEnumerable<Annonce>> GetAllAsync();
    Task<Annonce?> GetByIdAsync(int id);
    Task<Annonce> CreateAsync(Annonce annonce);
    Task UpdateAsync(Annonce annonce);
    Task DeleteAsync(int id);
}
