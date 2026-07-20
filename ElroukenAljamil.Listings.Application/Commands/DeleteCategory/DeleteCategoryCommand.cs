using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeleteCategory
{
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;
        public DeleteCategoryCommandHandler(ICategoryRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken ct)
        {
            var category = await _repository.GetByIdAsync(request.Id, ct);
            if (category is null) return false;

            await _repository.DeleteAsync(category, ct);
            return true;
        }
    }
}
