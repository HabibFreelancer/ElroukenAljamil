using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetCategoriesByMenu
{
    public record GetCategoriesByMenuQuery(int MenuId) : IRequest<List<CategoryDto>>;

    public class GetCategoriesByMenuQueryHandler : IRequestHandler<GetCategoriesByMenuQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        public GetCategoriesByMenuQueryHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<List<CategoryDto>> Handle(GetCategoriesByMenuQuery request, CancellationToken ct)
        {
            var categories = await _repository.GetByMenuIdAsync(request.MenuId, ct);
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                MenuId = c.MenuId,
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
