using ElroukenAljamil.Search.Application.DTOs;
using ElroukenAljamil.Search.Application.Queries.GetFacets;
using ElroukenAljamil.Search.Application.Queries.SearchListings;
using ElroukenAljamil.Search.Application.Queries.Suggest;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Search.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Recherche full-text avec filtres, tri et pagination.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(SearchResultDto), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 30, VaryByQueryKeys = new[]
        {
        "query", "category", "city", "minPrice", "maxPrice",
        "latitude", "longitude", "radiusKm", "sellerId", "sortBy", "page", "pageSize"
    })]
        public async Task<IActionResult> Search(
            [FromQuery] string? query,
            [FromQuery] string? category,
            [FromQuery] string? city,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusKm,
            [FromQuery] Guid? sellerId,
            [FromQuery] string sortBy = "relevance",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var searchQuery = new SearchListingsQuery
            {
                Query = query,
                Category = category,
                City = city,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                Latitude = latitude,
                Longitude = longitude,
                RadiusKm = radiusKm,
                SellerId = sellerId,
                SortBy = sortBy,
                Page = page,
                PageSize = pageSize
            };

            var result = await _mediator.Send(searchQuery, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Autocomplétion pour la barre de recherche.
        /// </summary>
        [HttpGet("suggest")]
        [ProducesResponseType(typeof(SuggestResponseDto), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 60, VaryByQueryKeys = new[] { "query", "max" })]
        public async Task<IActionResult> Suggest(
            [FromQuery] string query,
            [FromQuery] int max = 10,
            CancellationToken ct = default)
        {
            var suggestQuery = new SuggestQuery
            {
                Query = query,
                MaxSuggestions = max
            };

            var result = await _mediator.Send(suggestQuery, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Récupère les facettes (catégories avec compteurs, fourchette de prix).
        /// </summary>
        [HttpGet("facets")]
        [ProducesResponseType(typeof(FacetsResponseDto), StatusCodes.Status200OK)]
        [ResponseCache(Duration = 120, VaryByQueryKeys = new[] { "query", "category" })]
        public async Task<IActionResult> GetFacets(
            [FromQuery] string? query,
            [FromQuery] string? category,
            CancellationToken ct = default)
        {
            var facetsQuery = new GetFacetsQuery
            {
                Query = query,
                Category = category
            };

            var result = await _mediator.Send(facetsQuery, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
