using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.Login
{
    public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;

        public LoginCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(LoginCommand request, CancellationToken ct)
        {
            // Rechercher l'utilisateur par email
            var user = await _userRepository.GetByEmailAsync(request.Email.ToLowerInvariant(), ct);
            if (user is null)
                return Result<AuthResponseDto>.Failure("Email ou mot de passe incorrect.");

            // Vérifier si le compte est verrouillé
            if (user.IsLockedOut)
                return Result<AuthResponseDto>.Failure(
                    $"Compte verrouillé suite à trop de tentatives. Réessayez après {user.LockedUntil:HH:mm}.");

            // Vérifier le mot de passe
            if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                user.RecordFailedLogin();
                await _userRepository.UpdateAsync(user, ct);
                return Result<AuthResponseDto>.Failure("Email ou mot de passe incorrect.");
            }

            // Vérifier le statut du compte
            if (user.Status == Domain.Enums.UserStatus.Deactivated)
                return Result<AuthResponseDto>.Failure("Ce compte a été désactivé.");

            if (user.Status == Domain.Enums.UserStatus.Banned)
                return Result<AuthResponseDto>.Failure("Ce compte a été banni.");

            // Générer les tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpiration = _tokenService.GetRefreshTokenExpiration();

            user.RecordSuccessfulLogin(refreshToken, refreshExpiration);
            await _userRepository.UpdateAsync(user, ct);

            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
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
