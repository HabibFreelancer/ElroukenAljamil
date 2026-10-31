using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;

namespace ElroukenAljamil.Listings.Domain.Interfaces;

public interface IBugReportRepository
{
    Task<BugReport?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<(IEnumerable<BugReport> Items, int Total)> GetPagedAsync(
        int page, int pageSize,
        BugStatus? status = null,
        BugSeverity? severity = null,
        string? category = null,
        string? reportedByUserId = null,
        CancellationToken ct = default);
    Task AddAsync(BugReport bugReport, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}
