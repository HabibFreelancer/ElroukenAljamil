using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Commands.CreateListing;
using ElroukenAljamil.Listings.Application.Commands.DeactivateListing;
using ElroukenAljamil.Listings.Application.Commands.UpdateListing;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetListingById;
using ElroukenAljamil.Listings.Application.Queries.GetListings;
using ElroukenAljamil.Listings.Application.Queries.GetMyListings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;



namespace ElroukenAljamil.Listings.API.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public ListingsController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Récupère la liste paginée des annonces.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(PagedResult<ListingSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetListings(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            [FromQuery] string? category = null,
            [FromQuery] string? status = null,
            [FromQuery] Guid? sellerId = null,
            CancellationToken ct = default)
        {
            var query = new GetListingsQuery
            {
                Page = page,
                PageSize = pageSize,
                Category = category,
                Status = status,
                SellerId = sellerId
            };

            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Récupère une annonce par son identifiant.
        /// </summary>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ListingDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetListingByIdQuery(id), ct);

            if (!result.IsSuccess)
                return NotFound(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Récupère les annonces de l'utilisateur connecté.
        /// </summary>
        [HttpGet("mine")]
        [Authorize]
        [ProducesResponseType(typeof(List<ListingSummaryDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyListings(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMyListingsQuery(), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Crée une nouvelle annonce.
        /// </summary>
        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] CreateListingCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(nameof(GetById), new { id = result.Value }, result.Value);
        }

        /// <summary>
        /// Met à jour une annonce existante.
        /// </summary>
        [HttpPut("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateListingCommand command, CancellationToken ct)
        {
            // S'assurer que l'Id du path correspond au command
            var commandWithId = command with { Id = id };
            var result = await _mediator.Send(commandWithId, ct);

            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("introuvable"))
                    return NotFound(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }

            return NoContent();
        }

        /// <summary>
        /// Désactive une annonce.
        /// </summary>
        [HttpDelete("{id:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeactivateListingCommand(id), ct);

            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("introuvable"))
                    return NotFound(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }

            return NoContent();
        }
    }
}
