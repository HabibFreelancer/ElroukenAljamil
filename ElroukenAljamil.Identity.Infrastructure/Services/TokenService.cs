using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.BuildingBlocks.Security.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ElroukenAljamil.Identity.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly JwtSettings _jwtSettings;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IOptions<JwtSettings> jwtSettings, ILogger<TokenService> logger)
        {
            _jwtSettings = jwtSettings.Value;
            _logger = logger;
        }

        public string GenerateAccessToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Name, user.UserName),
                new(ClaimTypes.GivenName, user.FirstName),
                new(ClaimTypes.Surname, user.LastName),
                new(ClaimTypes.Role, user.Role.ToString()),
                new("full_name", user.FullName)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes), // Utilisation dynamique
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }

        public DateTime GetRefreshTokenExpiration()
        {
            // Utilisation directe de la propriété typée
            return DateTime.UtcNow.AddDays(_jwtSettings.RefreshTokenExpirationDays);
        }

        public string? GetUserIdFromExpiredToken(string token)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = false,
                ValidateIssuer = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret)),
                ValidateLifetime = false
            };

            try
            {
                var principal = new JwtSecurityTokenHandler()
                    .ValidateToken(token, tokenValidationParameters, out var securityToken);

                if (securityToken is not JwtSecurityToken jwtToken ||
                    !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    _logger.LogWarning("Token expiré invalide — algorithme de signature incorrect");
                    return null;
                }

                return principal.FindFirstValue(ClaimTypes.NameIdentifier);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Échec de la validation du token expiré");
                return null;
            }
        }
    }

}
