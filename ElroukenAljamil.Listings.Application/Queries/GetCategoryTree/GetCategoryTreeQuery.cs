using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetCategoryTree
{
    public record GetCategoryTreeQuery(int MenuId) : IRequest<List<CategoryDto>>;

    public class GetCategoryTreeQueryHandler : IRequestHandler<GetCategoryTreeQuery, List<CategoryDto>>
    {
        private readonly ICategoryRepository _repository;
        public GetCategoryTreeQueryHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<List<CategoryDto>> Handle(GetCategoryTreeQuery request, CancellationToken ct)
        {
            var roots = await _repository.GetTreeAsync(request.MenuId, ct);
            return roots.Select(MapWithChildren).ToList();
        }

        private static CategoryDto MapWithChildren(AnnonceCategory c) => new()
        {
            Id = c.Id,
            MenuId = c.MenuId,
            ParentCategoryId = c.ParentCategoryId,
            Name = c.Name,
            Slug = c.Slug,
            IsLink = c.IsLink,
            ShowInDeposit = c.ShowInDeposit,
            DisplayOrder = c.DisplayOrder,
            IsActive = c.IsActive,
            SubCategories = c.SubCategories.Select(MapWithChildren).ToList()
        };
    }
}
