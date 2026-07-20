using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Interfaces;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Identity.Infrastructure.Repositories
{
    public class VerificationCodeRepository : IVerificationCodeRepository
    {
        private readonly IdentityDbContext _context;
        public VerificationCodeRepository(IdentityDbContext context) => _context = context;

        public Task<VerificationCode?> GetLatestAsync(string target, CancellationToken ct = default) =>
            _context.VerificationCodes
                .Where(c => c.Target == target.ToLowerInvariant().Trim())
                .OrderByDescending(c => c.ExpiresAt)
                .FirstOrDefaultAsync(ct);

        public async Task UpsertAsync(VerificationCode code, CancellationToken ct = default)
        {
            var existing = await _context.VerificationCodes
                .Where(c => c.Target == code.Target)
                .ToListAsync(ct);
            _context.VerificationCodes.RemoveRange(existing);
            await _context.VerificationCodes.AddAsync(code, ct);
        }

        public Task SaveChangesAsync(CancellationToken ct = default) =>
            _context.SaveChangesAsync(ct);
    }
}
