using System.Security.Cryptography;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Identity.Application.Commands.SendEmailCode
{
    public record SendEmailCodeCommand(string Email) : IRequest<string>; // retourne maskedEmail

    public class SendEmailCodeCommandHandler : IRequestHandler<SendEmailCodeCommand, string>
    {
        private readonly IVerificationCodeRepository _codeRepository;
        private readonly ILogger<SendEmailCodeCommandHandler> _logger;

        public SendEmailCodeCommandHandler(IVerificationCodeRepository codeRepository,
            ILogger<SendEmailCodeCommandHandler> logger)
        {
            _codeRepository = codeRepository;
            _logger = logger;
        }

        public async Task<string> Handle(SendEmailCodeCommand request, CancellationToken ct)
        {
            var email = request.Email.ToLowerInvariant().Trim();
            var code = GenerateNumericCode(5);

            var verificationCode = VerificationCode.Create(email, code, expirationMinutes: 10);
            await _codeRepository.UpsertAsync(verificationCode, ct);
            await _codeRepository.SaveChangesAsync(ct);

            _logger.LogInformation("EMAIL VERIFICATION CODE | To: {Email} | Code: {Code}", email, code);

            return MaskEmail(email);
        }

        private static string GenerateNumericCode(int length)
        {
            var bytes = new byte[length];
            RandomNumberGenerator.Fill(bytes);
            return string.Concat(bytes.Select(b => (b % 10).ToString())).Substring(0, length);
        }

        private static string MaskEmail(string email)
        {
            var parts = email.Split('@');
            if (parts[0].Length <= 2) return email;
            return parts[0][0] + new string('*', parts[0].Length - 2) + parts[0][^1] + "@" + parts[1];
        }
    }
}
