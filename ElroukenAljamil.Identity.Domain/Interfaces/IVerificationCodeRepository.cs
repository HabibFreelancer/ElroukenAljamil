using ElroukenAljamil.Identity.Domain.Entities;

namespace ElroukenAljamil.Identity.Domain.Interfaces
{
    public interface IVerificationCodeRepository
    {
        Task<VerificationCode?> GetLatestAsync(string target, CancellationToken ct = default);
        Task UpsertAsync(VerificationCode code, CancellationToken ct = default);
        Task SaveChangesAsync(CancellationToken ct = default);
    }
}
