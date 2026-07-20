using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.VerifyEmailCode
{
    public record VerifyEmailCodeCommand(string Email, string Code) : IRequest<Result>;

    public class VerifyEmailCodeCommandHandler : IRequestHandler<VerifyEmailCodeCommand, Result>
    {
        private readonly IVerificationCodeRepository _codeRepository;
        public VerifyEmailCodeCommandHandler(IVerificationCodeRepository codeRepository) => _codeRepository = codeRepository;

        public async Task<Result> Handle(VerifyEmailCodeCommand request, CancellationToken ct)
        {
            var target = request.Email.ToLowerInvariant().Trim();
            var entry = await _codeRepository.GetLatestAsync(target, ct);

            if (entry is null || !entry.IsValid(request.Code))
                return Result.Failure("Code invalide ou expiré.");

            entry.MarkAsUsed();
            await _codeRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}
