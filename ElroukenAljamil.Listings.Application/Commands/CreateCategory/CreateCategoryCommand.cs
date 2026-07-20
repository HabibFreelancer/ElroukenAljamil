using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.CreateCategory
{
    public record CreateCategoryCommand(CreateCategoryRequest Request) : IRequest<CategoryDto>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _repository;
        public CreateCategoryCommandHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var category = ListingCategory.Create(r.MenuId, r.Name, r.Slug, r.DisplayOrder, r.ParentCategoryId, r.ShowInDeposit, r.IsLink);
            await _repository.AddAsync(category, ct);

            return new CategoryDto
            {
                Id = category.Id,
                MenuId = category.MenuId,
                ParentCategoryId = category.ParentCategoryId,
                Name = category.Name,
                Slug = category.Slug,
                IsLink = category.IsLink,
                ShowInDeposit = category.ShowInDeposit,
                DisplayOrder = category.DisplayOrder,
                IsActive = category.IsActive
            };
        }
    }
}
