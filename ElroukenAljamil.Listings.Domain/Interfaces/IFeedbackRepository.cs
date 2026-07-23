using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces;

public interface IFeedbackRepository
{
    Task<int> AddAsync(Feedback feedback, CancellationToken ct = default);
    Task<List<Feedback>> GetAllAsync(CancellationToken ct = default);
    Task<(int Total, List<(string Rating, int Count)> ByRating)> GetStatsAsync(CancellationToken ct = default);
}
