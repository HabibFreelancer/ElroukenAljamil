using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories;

public class BugReportRepository : IBugReportRepository
{
    private readonly ListingsDbContext _context;

    public BugReportRepository(ListingsDbContext context) => _context = context;

    public async Task<BugReport?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _context.BugReports
            .Include(b => b.Comments)
            .Include(b => b.Attachments)
            .FirstOrDefaultAsync(b => b.Id == id, ct);

    public async Task<(IEnumerable<BugReport> Items, int Total)> GetPagedAsync(
        int page, int pageSize,
        BugStatus? status = null,
        BugSeverity? severity = null,
        string? category = null,
        string? reportedByUserId = null,
        CancellationToken ct = default)
    {
        var query = _context.BugReports.AsQueryable();

        if (status.HasValue) query = query.Where(b => b.Status == status.Value);
        if (severity.HasValue) query = query.Where(b => b.Severity == severity.Value);
        if (!string.IsNullOrWhiteSpace(category)) query = query.Where(b => b.Category == category);
        if (!string.IsNullOrWhiteSpace(reportedByUserId)) query = query.Where(b => b.ReportedByUserId == reportedByUserId);

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(b => b.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task AddAsync(BugReport bugReport, CancellationToken ct = default)
        => await _context.BugReports.AddAsync(bugReport, ct);

    public async Task SaveChangesAsync(CancellationToken ct = default)
        => await _context.SaveChangesAsync(ct);
}
