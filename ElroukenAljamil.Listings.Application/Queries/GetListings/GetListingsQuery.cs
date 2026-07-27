using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetListings;

public record GetListingsQuery : IRequest<List<AnnonceDto>>;

public class GetListingsQueryHandler : IRequestHandler<GetListingsQuery, List<AnnonceDto>>
{
    private readonly IAnnonceRepository _repository;
    public GetListingsQueryHandler(IAnnonceRepository repository) => _repository = repository;

    public async Task<List<AnnonceDto>> Handle(GetListingsQuery request, CancellationToken ct)
    {
        var annonces = await _repository.GetAllAsync();
        return annonces
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new AnnonceDto
            {
                Id          = a.Id,
                Title       = a.Title,
                Description = a.Description,
                Price       = a.Price,
                CategoryId  = a.CategoryId,
                AdType      = a.AdType,
                Condition   = a.Condition,
                Location    = a.Location,
                Phone       = a.Phone,
                Email       = a.Email,
                HidePhone   = a.HidePhone,
                ExtraData   = a.ExtraData,
                Status      = a.Status,
                UserId      = a.UserId,
                CreatedAt   = a.CreatedAt
            }).ToList();
    }
}
