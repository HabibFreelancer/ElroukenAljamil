using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.UpdateMenu
{
    public record UpdateMenuCommand(int Id, UpdateMenuRequest Request) : IRequest<bool>;

    public class UpdateMenuCommandHandler : IRequestHandler<UpdateMenuCommand, bool>
    {
        private readonly IMenuRepository _repository;
        public UpdateMenuCommandHandler(IMenuRepository repository) => _repository = repository;

        public async Task<bool> Handle(UpdateMenuCommand request, CancellationToken ct)
        {
            var menu = await _repository.GetByIdAsync(request.Id, ct);
            if (menu is null) return false;

            var r = request.Request;
            menu.Update(r.Name, r.Slug, r.DisplayOrder, r.Icon, r.IsActive);
            await _repository.UpdateAsync(menu, ct);
            return true;
        }
    }
}
