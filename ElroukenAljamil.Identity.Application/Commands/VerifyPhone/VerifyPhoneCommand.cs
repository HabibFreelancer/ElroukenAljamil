using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.VerifyPhone
{
    public record VerifyPhoneCommand(string Email, string Code) : IRequest<Result<AuthResponseDto>>;

    public class VerifyPhoneCommandHandler : IRequestHandler<VerifyPhoneCommand, Result<AuthResponseDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeRepository _codeRepository;
        private readonly ITokenService _tokenService;

        public VerifyPhoneCommandHandler(IUserRepository userRepository,
            IVerificationCodeRepository codeRepository,
            ITokenService tokenService)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _tokenService = tokenService;
        }

        public async Task<Result<AuthResponseDto>> Handle(VerifyPhoneCommand request, CancellationToken ct)
        {
            var email = request.Email.ToLowerInvariant().Trim();
            var target = $"phone_{email}";

            var entry = await _codeRepository.GetLatestAsync(target, ct);
            if (entry is null || !entry.IsValid(request.Code))
                return Result<AuthResponseDto>.Failure("Code invalide ou expiré.");

            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user is null) return Result<AuthResponseDto>.Failure("Utilisateur non trouvé.");

            entry.MarkAsUsed();
            user.VerifyEmail(); // active le compte

            var accessToken = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            user.RecordSuccessfulLogin(refreshToken, _tokenService.GetRefreshTokenExpiration());

            await _userRepository.UpdateAsync(user, ct);
            await _codeRepository.SaveChangesAsync(ct);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = DateTime.UtcNow.AddMinutes(60),
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
            });
        }
    }
}
