using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetCategoriesForDeposit
{
    public record GetCategoriesForDepositQuery(int MenuId) : IRequest<List<CategoryDto>>;

    public class GetCategoriesForDepositQueryHandler : IRequestHandler<GetCategoriesForDepositQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        public GetCategoriesForDepositQueryHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<List<CategoryDto>> Handle(GetCategoriesForDepositQuery request, CancellationToken ct)
        {
            var categories = await _repository.GetForDepositAsync(request.MenuId, ct);
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
