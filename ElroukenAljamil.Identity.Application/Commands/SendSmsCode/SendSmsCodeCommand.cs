using System.Security.Cryptography;
using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Identity.Application.Commands.SendSmsCode
{
    public record SendSmsCodeCommand(string Email, string Phone) : IRequest<Result>;

    public class SendSmsCodeCommandHandler : IRequestHandler<SendSmsCodeCommand, Result>
    {
        private readonly IUserRepository _userRepository;
        private readonly IVerificationCodeRepository _codeRepository;
        private readonly ILogger<SendSmsCodeCommandHandler> _logger;

        public SendSmsCodeCommandHandler(IUserRepository userRepository,
            IVerificationCodeRepository codeRepository,
            ILogger<SendSmsCodeCommandHandler> logger)
        {
            _userRepository = userRepository;
            _codeRepository = codeRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(SendSmsCodeCommand request, CancellationToken ct)
        {
            var email = request.Email.ToLowerInvariant().Trim();
            var user = await _userRepository.GetByEmailAsync(email, ct);
            if (user is null) return Result.Failure("Utilisateur non trouvé.");

            user.UpdateProfile(user.FirstName, user.LastName, request.Phone, user.Address);
            await _userRepository.UpdateAsync(user, ct);

            var code = GenerateNumericCode(6);
            var target = $"phone_{email}";
            var verificationCode = VerificationCode.Create(target, code, expirationMinutes: 10);
            await _codeRepository.UpsertAsync(verificationCode, ct);
            await _codeRepository.SaveChangesAsync(ct);

            _logger.LogInformation("SMS VERIFICATION CODE | To: {Phone} | Code: {Code}", request.Phone, code);

            return Result.Success();
        }

        private static string GenerateNumericCode(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return string.Concat(bytes.Select(b => (b % 10).ToString())).Substring(0, length);
        }
    }
}
