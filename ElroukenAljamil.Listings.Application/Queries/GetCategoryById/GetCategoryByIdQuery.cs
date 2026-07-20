using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetCategoryById
{
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _repository;
        public GetCategoryByIdQueryHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken ct)
        {
            var c = await _repository.GetByIdAsync(request.Id, ct);
            if (c is null) return null;

            return new CategoryDto
            {
                Id = c.Id,
                MenuId = c.MenuId,
                MenuName = c.Menu?.Name ?? string.Empty,
                MenuIcon = c.Menu?.Icon,
                ParentCategoryId = c.ParentCategoryId,
                Name = c.Name,
                Slug = c.Slug,
                IsLink = c.IsLink,
                ShowInDeposit = c.ShowInDeposit,
                DisplayOrder = c.DisplayOrder,
                IsActive = c.IsActive
            };
        }
    }
}
