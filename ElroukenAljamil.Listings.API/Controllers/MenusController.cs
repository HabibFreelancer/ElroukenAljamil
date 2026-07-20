using ElroukenAljamil.Listings.Application.Commands.CreateMenu;
using ElroukenAljamil.Listings.Application.Commands.DeleteMenu;
using ElroukenAljamil.Listings.Application.Commands.UpdateMenu;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetMenus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MenusController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MenusController(IMediator mediator) => _mediator = mediator;

        /// <summary>Tous les menus actifs.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMenusQuery(), ct);
            return Ok(result);
        }

        /// <summary>Créer un menu (admin).</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateMenuRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateMenuCommand(request), ct);
            return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
        }

        /// <summary>Modifier un menu (admin).</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMenuRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateMenuCommand(id, request), ct);
            return success ? NoContent() : NotFound();
        }

        /// <summary>Supprimer un menu (admin).</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteMenuCommand(id), ct);
            return success ? NoContent() : NotFound();
        }
    }
}
