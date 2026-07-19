using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories
{
    public abstract class BaseRepository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly NotificationDbContext Context;
        protected readonly DbSet<T> DbSet;

        protected BaseRepository(NotificationDbContext context)
        {
            Context = context;
            DbSet = context.Set<T>();
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken ct = default)
            => await DbSet.FindAsync(new object[] { id }, ct);

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken ct = default)
            => await DbSet.ToListAsync(ct);

        public async Task<IReadOnlyList<T>> FindAsync(Expression<Func<T, bool>> predicate, CancellationToken ct = default)
            => await DbSet.Where(predicate).ToListAsync(ct);

        public async Task<T> AddAsync(T entity, CancellationToken ct = default)
        {
            await DbSet.AddAsync(entity, ct);
            await Context.SaveChangesAsync(ct);
            return entity;
        }

        public async Task UpdateAsync(T entity, CancellationToken ct = default)
        {
            DbSet.Update(entity);
            await Context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(T entity, CancellationToken ct = default)
        {
            DbSet.Remove(entity);
            await Context.SaveChangesAsync(ct);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken ct = default)
            => await DbSet.AnyAsync(e => e.Id == id, ct);

        public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken ct = default)
            => predicate is null ? await DbSet.CountAsync(ct) : await DbSet.CountAsync(predicate, ct);
    }
}
