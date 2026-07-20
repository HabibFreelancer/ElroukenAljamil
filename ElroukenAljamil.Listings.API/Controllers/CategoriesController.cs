using ElroukenAljamil.Listings.Application.Commands.CreateCategory;
using ElroukenAljamil.Listings.Application.Commands.DeleteCategory;
using ElroukenAljamil.Listings.Application.Commands.UpdateCategory;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetCategories;
using ElroukenAljamil.Listings.Application.Queries.GetCategoriesByMenu;
using ElroukenAljamil.Listings.Application.Queries.GetCategoriesForDeposit;
using ElroukenAljamil.Listings.Application.Queries.GetCategoryById;
using ElroukenAljamil.Listings.Application.Queries.GetCategoryTree;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator) => _mediator = mediator;

        /// <summary>Toutes les catégories avec leur menu.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoriesQuery(), ct);
            return Ok(result);
        }

        /// <summary>Détail d'une catégorie.</summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoryByIdQuery(id), ct);
            return result is null ? NotFound() : Ok(result);
        }

        /// <summary>Catégories actives d'un menu.</summary>
        [HttpGet("by-menu/{menuId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByMenu(int menuId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoriesByMenuQuery(menuId), ct);
            return Ok(result);
        }

        /// <summary>Catégories ShowInDeposit d'un menu (pour le formulaire de dépôt).</summary>
        [HttpGet("for-deposit/{menuId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetForDeposit(int menuId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoriesForDepositQuery(menuId), ct);
            return Ok(result);
        }

        /// <summary>Arbre hiérarchique parent/enfant d'un menu.</summary>
        [HttpGet("tree/{menuId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTree(int menuId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetCategoryTreeQuery(menuId), ct);
            return Ok(result);
        }

        /// <summary>Créer une catégorie (admin).</summary>
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateCategoryCommand(request), ct);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>Modifier une catégorie (admin).</summary>
        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateCategoryCommand(id, request), ct);
            return success ? NoContent() : NotFound();
        }

        /// <summary>Supprimer une catégorie (admin).</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteCategoryCommand(id), ct);
            return success ? NoContent() : NotFound();
        }
    }
}
