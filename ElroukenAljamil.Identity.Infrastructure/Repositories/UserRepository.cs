using System.Linq.Expressions;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Interfaces;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Identity.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly IdentityDbContext _context;

        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<ApplicationUser?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Id == id, ct);
        }

        public async Task<ApplicationUser?> GetByEmailAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Email == email.ToLowerInvariant(), ct);
        }

        public async Task<ApplicationUser?> GetByUserNameAsync(string userName, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.UserName == userName, ct);
        }

        public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken, CancellationToken ct = default)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == refreshToken, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Email == email.ToLowerInvariant(), ct);
        }

        public async Task<bool> UserNameExistsAsync(string userName, CancellationToken ct = default)
        {
            return await _context.Users
                .AnyAsync(u => u.UserName == userName, ct);
        }

        public async Task AddAsync(ApplicationUser entity, CancellationToken ct = default)
        {
            await _context.Users.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(ApplicationUser entity, CancellationToken ct = default)
        {
            _context.Users.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(ApplicationUser entity, CancellationToken ct = default)
        {
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task<IReadOnlyList<ApplicationUser>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.Users.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<ApplicationUser>> FindAsync(Expression<Func<ApplicationUser, bool>> predicate, CancellationToken ct = default)
        {
            return await _context.Users
                .Where(predicate)
                .ToListAsync(ct);
        }

        async Task<ApplicationUser> IRepository<ApplicationUser>.AddAsync(ApplicationUser entity, CancellationToken ct)
        {
            var entry = await _context.Users.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
            return entry.Entity;
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Users
                .AnyAsync(u => u.Id == id, ct);
        }

        public async Task<int> CountAsync(Expression<Func<ApplicationUser, bool>>? predicate = null, CancellationToken ct = default)
        {
            if (predicate == null)
            {
                return await _context.Users.CountAsync(ct);
            }
            return await _context.Users.CountAsync(predicate, ct);
        }
    }

}
