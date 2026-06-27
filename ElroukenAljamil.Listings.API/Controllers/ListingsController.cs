using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ElroukenAljamil.Listings.Application.Commands;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries;
using System.Security.Claims;


namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    [Produces("application/json")]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ListingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupère une annonce par son identifiant.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetListingByIdQuery(id), cancellationToken);

            return result is not null ? Ok(result) : NotFound();
        }

        /// <summary>
        /// Récupère les annonces actives d'une catégorie (paginé).
        /// </summary>
        [HttpGet("category/{categoryId:guid}")]
        [ProducesResponseType(typeof(IReadOnlyList<ListingDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByCategory(
            Guid categoryId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(
                new GetListingsByCategoryQuery(categoryId, page, pageSize), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Crée une nouvelle annonce.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(ListingDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] CreateListingRequest request,
            CancellationToken cancellationToken)
        {
            var sellerId = GetCurrentUserId();

            var command = new CreateListingCommand(
                Title: request.Title,
                Description: request.Description,
                Price: request.Price,
                Currency: request.Currency,
                SellerId: sellerId,
                CategoryId: request.CategoryId,
                City: request.City,
                PostalCode: request.PostalCode,
                Country: request.Country
            );

            var result = await _mediator.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Publie une annonce (la rend visible).
        /// </summary>
        [HttpPost("{id:guid}/publish")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
        {
            await _mediator.Send(new PublishListingCommand(id), cancellationToken);
            return NoContent();
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

}
