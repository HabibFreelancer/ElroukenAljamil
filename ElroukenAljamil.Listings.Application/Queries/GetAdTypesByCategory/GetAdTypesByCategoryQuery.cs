using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetAdTypesByCategory;

public record GetAdTypesByCategoryQuery(int CategoryId) : IRequest<List<AdTypeDto>>;

public class GetAdTypesByCategoryQueryHandler : IRequestHandler<GetAdTypesByCategoryQuery, List<AdTypeDto>>
{
    private readonly IAdTypeRepository _repository;
    public GetAdTypesByCategoryQueryHandler(IAdTypeRepository repository) => _repository = repository;

    public async Task<List<AdTypeDto>> Handle(GetAdTypesByCategoryQuery request, CancellationToken ct)
    {
        var adTypes = await _repository.GetByCategoryWithFallbackAsync(request.CategoryId, ct);
        return adTypes.Select(a => new AdTypeDto
        {
            Id           = a.Id,
            CategoryId   = a.CategoryId,
            CategoryName = a.Category?.Name ?? string.Empty,
            Label        = a.Label,
            Description  = a.Description,
            IsDefault    = a.IsDefault,
            DisplayOrder = a.DisplayOrder,
            IsActive     = a.IsActive
        }).ToList();
    }
}
