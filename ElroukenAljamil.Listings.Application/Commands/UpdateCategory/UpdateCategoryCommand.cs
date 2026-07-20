using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.UpdateCategory
{
    public record UpdateCategoryCommand(int Id, UpdateCategoryRequest Request) : IRequest<bool>;

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;
        public UpdateCategoryCommandHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateCategoryCommand request, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(request.Id, ct);
            if (category is null) return false;

            var r = request.Request;
            category.Update(r.Name, r.Slug, r.DisplayOrder, r.ParentCategoryId, r.ShowInDeposit, r.IsLink, r.IsActive);
            await _repository.UpdateAsync(category, ct);
            return true;
        }
    }
}
