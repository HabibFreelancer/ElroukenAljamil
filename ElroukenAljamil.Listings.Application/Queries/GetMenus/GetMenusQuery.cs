using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetMenus
{
    public record GetMenusQuery : IRequest<List<MenuDto>>;

    public class GetMenusQueryHandler : IRequestHandler<GetMenusQuery, List<MenuDto>>
    {
        private readonly IMenuRepository _repository;
        public GetMenusQueryHandler(IMenuRepository repository) => _repository = repository;

        public async Task<List<MenuDto>> Handle(GetMenusQuery request, CancellationToken ct)
        {
            var menus = await _repository.GetAllAsync(ct);
            return menus.Select(m => new MenuDto
            {
                Id = m.Id,
                Name = m.Name,
                Slug = m.Slug,
                Icon = m.Icon,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive
            }).ToList();
        }
    }
}
