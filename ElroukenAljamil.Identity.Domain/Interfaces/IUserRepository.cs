using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;

namespace ElroukenAljamil.Identity.Domain.Interfaces
{
    /// <summary>
    /// Repository spécifique aux utilisateurs, étend le contrat générique.
    /// </summary>
    public interface IUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default);
        Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken ct = default);
        Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct = default);
        Task<bool> UserNameExistsAsync(string userName, CancellationToken ct = default);
    }
}
