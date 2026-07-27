using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetMyListings;

public record GetMyListingsQuery(
    string UserId,
    string? Search,
    string? Status,
    string? SortBy) : IRequest<List<AnnonceDto>>;

public class GetMyListingsQueryHandler : IRequestHandler<GetMyListingsQuery, List<AnnonceDto>>
{
    private readonly IAnnonceRepository _repository;
    public GetMyListingsQueryHandler(IAnnonceRepository repository) => _repository = repository;

    public async Task<List<AnnonceDto>> Handle(GetMyListingsQuery request, CancellationToken ct)
    {
        var annonces = await _repository.GetByUserIdAsync(
            request.UserId, request.Search, request.Status, request.SortBy);

        return annonces.Select(a => new AnnonceDto
        {
            Id          = a.Id,
            Title       = a.Title,
            Description = a.Description,
            Price       = a.Price,
            CategoryId  = a.CategoryId,
            AdType      = a.AdType,
            Location    = a.Location,
            Status      = a.Status,
            UserId      = a.UserId,
            CreatedAt   = a.CreatedAt
        }).ToList();
    }
}
