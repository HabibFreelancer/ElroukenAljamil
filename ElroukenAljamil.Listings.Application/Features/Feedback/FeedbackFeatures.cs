using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Features.Feedback;

// ── DTOs ─────────────────────────────────────────────────────────────────────

public class CreateFeedbackRequest
{
    public int? AnnonceId { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string Rating { get; set; } = string.Empty;
    public string? Category { get; set; }
}

public class FeedbackDto
{
    public int Id { get; set; }
    public int? AnnonceId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
    public string Rating { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class RatingCount
{
    public string Rating { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class FeedbackStatsDto
{
    public int Total { get; set; }
    public IEnumerable<RatingCount> ByRating { get; set; } = [];
}

// ── Queries ───────────────────────────────────────────────────────────────────

public record GetAllFeedbacksQuery : IRequest<IEnumerable<FeedbackDto>>;

public class GetAllFeedbacksQueryHandler : IRequestHandler<GetAllFeedbacksQuery, IEnumerable<FeedbackDto>>
{
    private readonly IFeedbackRepository _repo;
    public GetAllFeedbacksQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<IEnumerable<FeedbackDto>> Handle(GetAllFeedbacksQuery request, CancellationToken ct)
    {
        var feedbacks = await _repo.GetAllAsync(ct);
        return feedbacks.Select(f => new FeedbackDto
        {
            Id        = f.Id,
            AnnonceId = f.AnnonceId,
            UserId    = f.UserId,
            UserEmail = f.UserEmail,
            Rating    = f.Rating,
            Category  = f.Category,
            CreatedAt = f.CreatedAt
        });
    }
}

public record GetFeedbackStatsQuery : IRequest<FeedbackStatsDto>;

public class GetFeedbackStatsQueryHandler : IRequestHandler<GetFeedbackStatsQuery, FeedbackStatsDto>
{
    private readonly IFeedbackRepository _repo;
    public GetFeedbackStatsQueryHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<FeedbackStatsDto> Handle(GetFeedbackStatsQuery request, CancellationToken ct)
    {
        var (total, byRating) = await _repo.GetStatsAsync(ct);
        return new FeedbackStatsDto
        {
            Total    = total,
            ByRating = byRating.Select(x => new RatingCount { Rating = x.Rating, Count = x.Count })
        };
    }
}

// ── Commands ──────────────────────────────────────────────────────────────────

public record CreateFeedbackCommand(CreateFeedbackRequest Request) : IRequest<int>;

public class CreateFeedbackCommandHandler : IRequestHandler<CreateFeedbackCommand, int>
{
    private readonly IFeedbackRepository _repo;
    public CreateFeedbackCommandHandler(IFeedbackRepository repo) => _repo = repo;

    public async Task<int> Handle(CreateFeedbackCommand command, CancellationToken ct)
    {
        var req = command.Request;
        var feedback = new ElroukenAljamil.Listings.Domain.Entities.Feedback
        {
            AnnonceId = req.AnnonceId,
            UserId    = req.UserId ?? string.Empty,
            UserEmail = req.UserEmail ?? string.Empty,
            Rating    = req.Rating,
            Category  = req.Category ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };
        return await _repo.AddAsync(feedback, ct);
    }
}
