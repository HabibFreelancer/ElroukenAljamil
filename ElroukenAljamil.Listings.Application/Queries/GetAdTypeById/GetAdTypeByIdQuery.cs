using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetAdTypeById
{
    public record GetAdTypeByIdQuery(int Id) : IRequest<AdTypeDto?>;

    public class GetAdTypeByIdQueryHandler : IRequestHandler<GetAdTypeByIdQuery, AdTypeDto?>
    {
        private readonly IAdTypeRepository _repository;
        public GetAdTypeByIdQueryHandler(IAdTypeRepository repository) => _repository = repository;

        public async Task<AdTypeDto?> Handle(GetAdTypeByIdQuery request, CancellationToken ct)
        {
            var a = await _repository.GetByIdAsync(request.Id, ct);
            if (a is null) return null;

            return new AdTypeDto
            {
                Id = a.Id,
                CategoryId = a.CategoryId,
                CategoryName = a.Category?.Name ?? string.Empty,
                Label = a.Label,
                Description = a.Description,
                IsDefault = a.IsDefault,
                DisplayOrder = a.DisplayOrder,
                IsActive = a.IsActive
            };
        }
    }
}
