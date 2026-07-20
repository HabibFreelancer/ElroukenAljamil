using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeleteMenu
{
    public record DeleteMenuCommand(int Id) : IRequest<bool>;

    public class DeleteMenuCommandHandler : IRequestHandler<DeleteMenuCommand, bool>
    {
        private readonly IMenuRepository _repository;
        public DeleteMenuCommandHandler(IMenuRepository repository) => _repository = repository;

        public async Task<bool> Handle(DeleteMenuCommand request, CancellationToken ct)
        {
            var menu = await _repository.GetByIdAsync(request.Id, ct);
            if (menu is null) return false;

            await _repository.DeleteAsync(menu, ct);
            return true;
        }
    }
}
