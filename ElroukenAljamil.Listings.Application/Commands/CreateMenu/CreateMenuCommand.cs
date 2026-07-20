using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.CreateMenu
{
    public record CreateMenuCommand(CreateMenuRequest Request) : IRequest<MenuDto>;

    public class CreateMenuCommandHandler : IRequestHandler<CreateMenuCommand, MenuDto>
    {
        private readonly IMenuRepository _repository;
        public CreateMenuCommandHandler(IMenuRepository repository) => _repository = repository;

        public async Task<MenuDto> Handle(CreateMenuCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var menu = ListingMenu.Create(r.Name, r.Slug, r.DisplayOrder, r.Icon);
            await _repository.AddAsync(menu, ct);

            return new MenuDto
            {
                Id = menu.Id,
                Name = menu.Name,
                Slug = menu.Slug,
                Icon = menu.Icon,
                DisplayOrder = menu.DisplayOrder,
                IsActive = menu.IsActive
            };
        }
    }
}
