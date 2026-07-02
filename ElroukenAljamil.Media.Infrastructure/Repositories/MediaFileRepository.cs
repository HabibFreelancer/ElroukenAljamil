using System.Linq.Expressions;
using ElroukenAljamil.BuildingBlocks.Common.Interfaces;
using ElroukenAljamil.Media.Domain.Entities;
using ElroukenAljamil.Media.Domain.Enums;
using ElroukenAljamil.Media.Domain.Interfaces;
using ElroukenAljamil.Media.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Media.Infrastructure.Repositories
{
    public class MediaFileRepository : IMediaFileRepository
    {
        private readonly MediaDbContext _context;

        public MediaFileRepository(MediaDbContext context)
        {
            _context = context;
        }

        public async Task<MediaFile?> GetByIdAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.MediaFiles.FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<IReadOnlyList<MediaFile>> GetAllAsync(CancellationToken ct = default)
        {
            return await _context.MediaFiles.ToListAsync(ct);
        }

        public async Task<IReadOnlyList<MediaFile>> GetByOwnerIdAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _context.MediaFiles
                .Where(m => m.OwnerId == ownerId)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<MediaFile>> GetByListingIdAsync(Guid listingId, CancellationToken ct = default)
        {
            return await _context.MediaFiles
                .Where(m => m.ListingId == listingId)
                .OrderBy(m => m.SortOrder)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<MediaFile>> GetByStatusAsync(MediaStatus status, CancellationToken ct = default)
        {
            return await _context.MediaFiles
                .Where(m => m.Status == status)
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<MediaFile>> GetOrphanedFilesAsync(TimeSpan olderThan, CancellationToken ct = default)
        {
            var cutoff = DateTime.UtcNow - olderThan;
            return await _context.MediaFiles
                .Where(m => m.ListingId == null &&
                            m.CreatedAt < cutoff &&
                            m.Status != MediaStatus.MarkedForDeletion)
                .ToListAsync(ct);
        }

        public async Task<int> GetCountByListingAsync(Guid listingId, CancellationToken ct = default)
        {
            return await _context.MediaFiles
                .CountAsync(m => m.ListingId == listingId, ct);
        }

        public async Task<int> GetCountByOwnerAsync(Guid ownerId, CancellationToken ct = default)
        {
            return await _context.MediaFiles
                .CountAsync(m => m.OwnerId == ownerId, ct);
        }

        public async Task AddAsync(MediaFile entity, CancellationToken ct = default)
        {
            await _context.MediaFiles.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(MediaFile entity, CancellationToken ct = default)
        {
            _context.MediaFiles.Update(entity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(MediaFile entity, CancellationToken ct = default)
        {
            _context.MediaFiles.Remove(entity);
            await _context.SaveChangesAsync(ct);
        }

        Task<IReadOnlyList<MediaFile>> IRepository<MediaFile>.FindAsync(Expression<Func<MediaFile, bool>> predicate, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<MediaFile> IRepository<MediaFile>.AddAsync(MediaFile entity, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<bool> IRepository<MediaFile>.ExistsAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        Task<int> IRepository<MediaFile>.CountAsync(Expression<Func<MediaFile, bool>>? predicate, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}
