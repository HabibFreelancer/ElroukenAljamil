using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories;

public class FeedbackRepository : IFeedbackRepository
{
    private readonly ListingsDbContext _context;

    public FeedbackRepository(ListingsDbContext context)
    {
        _context = context;
    }

    public async Task<int> AddAsync(Feedback feedback, CancellationToken ct = default)
    {
        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync(ct);
        return feedback.Id;
    }

    public async Task<List<Feedback>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync(ct);
    }

    public async Task<(int Total, List<(string Rating, int Count)> ByRating)> GetStatsAsync(CancellationToken ct = default)
    {
        var total = await _context.Feedbacks.CountAsync(ct);

        var byRating = await _context.Feedbacks
            .GroupBy(f => f.Rating)
            .Select(g => new { Rating = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        return (total, byRating.Select(x => (x.Rating, x.Count)).ToList());
    }
}
