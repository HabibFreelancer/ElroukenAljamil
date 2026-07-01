using ElroukenAljamil.Identity.Domain.Entities;

namespace ElroukenAljamil.Identity.Application.Interfaces
{
    /// <summary>
    /// Service de génération et validation des tokens JWT.
    /// </summary>
    public interface ITokenService
    {
        string GenerateAccessToken(ApplicationUser user);
        string GenerateRefreshToken();
        DateTime GetRefreshTokenExpiration();
        string? GetUserIdFromExpiredToken(string token);
    }
}
