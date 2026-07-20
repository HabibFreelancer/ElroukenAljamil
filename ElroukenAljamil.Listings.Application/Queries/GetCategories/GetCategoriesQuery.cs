using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetCategories
{
    public record GetCategoriesQuery : IRequest<List<CategoryDto>>;

    public class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        public GetCategoriesQueryHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
        {
            var categories = await _repository.GetAllAsync(ct);
            return categories.Select(c => new CategoryDto
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
            }).ToList();
        }
    }
}
