using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetAdTypes
{
    public record GetAdTypesQuery : IRequest<List<AdTypeDto>>;

    public class GetAdTypesQueryHandler : IRequestHandler<GetAdTypesQuery, List<AdTypeDto>>
    {
        private readonly IAdTypeRepository _repository;
        public GetAdTypesQueryHandler(IAdTypeRepository repository) => _repository = repository;

        public async Task<List<AdTypeDto>> Handle(GetAdTypesQuery request, CancellationToken ct)
        {
            var adTypes = await _repository.GetAllAsync(ct);
            return adTypes.Select(a => new AdTypeDto
            {
                Id = a.Id,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? string.Empty,
                Label = a.Label,
                Description = a.Description,
                IsDefault = a.IsDefault,
                DisplayOrder = a.DisplayOrder,
                IsActive = a.IsActive
            }).ToList();
        }
    }
}
