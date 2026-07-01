using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Identity;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.Register
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenService _tokenService;
        private readonly IEventBus _eventBus;

        public RegisterCommandHandler(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ITokenService tokenService,
            IEventBus eventBus)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _tokenService = tokenService;
            _eventBus = eventBus;
        }

        public async Task<Result<AuthResponseDto>> Handle(RegisterCommand request, CancellationToken ct)
        {
            // Vérifier l'unicité de l'email
            if (await _userRepository.EmailExistsAsync(request.Email, ct))
                return Result<AuthResponseDto>.Failure("Cet email est déjà utilisé.");

            // Vérifier l'unicité du nom d'utilisateur
            if (await _userRepository.UserNameExistsAsync(request.UserName, ct))
                return Result<AuthResponseDto>.Failure("Ce nom d'utilisateur est déjà pris.");

            // Hasher le mot de passe
            var passwordHash = _passwordHasher.Hash(request.Password);

            // Créer l'utilisateur via la factory du domaine
            var user = ApplicationUser.Create(
                email: request.Email,
                userName: request.UserName,
                passwordHash: passwordHash,
                firstName: request.FirstName,
                lastName: request.LastName,
                phoneNumber: request.PhoneNumber);

            // Générer les tokens
            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var refreshExpiration = _tokenService.GetRefreshTokenExpiration();

            user.RecordSuccessfulLogin(refreshToken, refreshExpiration);

            // Persister
            await _userRepository.AddAsync(user, ct);

            // Publier l'événement d'intégration
            await _eventBus.PublishAsync(new UserRegisteredEvent
            {
                UserId = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RegisteredAt = DateTime.UtcNow
            }, ct);

            // Construire la réponse
            var response = new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddHours(1),
                User = MapToProfile(user)
            };

            return Result<AuthResponseDto>.Success(response);
        }

        private static UserProfileDto MapToProfile(ApplicationUser user) => new()
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
        };
    }

}
