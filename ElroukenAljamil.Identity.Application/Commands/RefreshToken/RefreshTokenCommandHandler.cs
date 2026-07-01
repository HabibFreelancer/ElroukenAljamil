using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;

        public RefreshTokenCommandHandler(
            IUserRepository userRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(RefreshTokenCommand request, CancellationToken ct)
        {
            // Extraire le UserId depuis le token expiré
            var userId = _tokenService.GetUserIdFromExpiredToken(request.AccessToken);
            if (userId is null)
                return Result<AuthResponseDto>.Failure("Token invalide.");

            var user = await _userRepository.GetByIdAsync(Guid.Parse(userId), ct);
            if (user is null)
                return Result<AuthResponseDto>.Failure("Utilisateur introuvable.");

            // Valider le refresh token
            if (!user.IsRefreshTokenValid(request.RefreshToken))
                return Result<AuthResponseDto>.Failure("Refresh token invalide ou expiré.");

            // Générer de nouveaux tokens (rotation)
            var newAccessToken = _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpiration = _tokenService.GetRefreshTokenExpiration();

            user.RecordSuccessfulLogin(newRefreshToken, refreshExpiration);
            await _userRepository.UpdateAsync(user, ct);

            var response = new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = new UserProfileDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    UserName = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Role = user.Role.ToString(),
                    Status = user.Status.ToString(),
                    AvatarUrl = user.AvatarUrl,
                    CreatedAt = user.CreatedAt
                }
            };

            return Result<AuthResponseDto>.Success(response);
        }
    }
}
