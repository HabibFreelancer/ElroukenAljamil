using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Commands.DeleteAnnonce;
using ElroukenAljamil.Listings.Application.Commands.PauseAnnonce;
using ElroukenAljamil.Listings.Application.Commands.ToggleFavorite;
using ElroukenAljamil.Listings.Application.Commands.TrackView;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetAdTypesByCategory;
using ElroukenAljamil.Listings.Application.Queries.GetListingById;
using ElroukenAljamil.Listings.Application.Queries.GetListings;
using ElroukenAljamil.Listings.Application.Queries.GetMyListings;
using ElroukenAljamil.Listings.Application.Queries.GetPriceEstimate;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ListingsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly ListingsDbContext _db;

        public ListingsController(IMediator mediator, ICurrentUserService currentUser, ListingsDbContext db)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _db = db;
        }

        /// <summary>Estimation de prix basée sur les annonces similaires.</summary>
        [HttpPost("price-estimate")]
        [AllowAnonymous]
        public async Task<IActionResult> GetPriceEstimate([FromBody] PriceEstimateRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(
                new GetPriceEstimateQuery(request.CategoryId, request.Brand, request.Model), ct);
            return Ok(result);
        }

        /// <summary>Types d'annonces pour une catégorie (avec fallback sur la catégorie parente).</summary>
        [HttpGet("adtypes/{categoryId:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAdTypes(int categoryId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAdTypesByCategoryQuery(categoryId), ct);
            return Ok(result);
        }

        /// <summary>Détail d'une annonce par id.</summary>
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetListingByIdQuery(id), ct);
            if (result == null) return NotFound();
            return Ok(result);
        }

        /// <summary>Supprime une annonce.</summary>
        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteAnnonce(int id, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteAnnonceCommand(id), ct);
            if (!deleted) return NotFound();
            return NoContent();
        }

        /// <summary>Pause / reprend une annonce (toggle published ↔ paused).</summary>
        [HttpPut("{id:int}/pause")]
        [Authorize]
        public async Task<IActionResult> PauseAnnonce(int id, CancellationToken ct)
        {
            var status = await _mediator.Send(new PauseAnnonceCommand(id), ct);
            if (status == null) return NotFound();
            return Ok(new { status });
        }

        /// <summary>Toggle favori sur une annonce (authentifié requis).</summary>
        [HttpPost("{id:int}/favorite")]
        [Authorize]
        public async Task<IActionResult> ToggleFavorite(int id, CancellationToken ct)
        {
            var userId = _currentUser.UserId.ToString();
            var favorited = await _mediator.Send(new ToggleFavoriteCommand(id, userId), ct);
            return Ok(new { favorited });
        }

        /// <summary>Enregistre une vue sur une annonce.</summary>
        [HttpPost("{id:int}/view")]
        [AllowAnonymous]
        public async Task<IActionResult> TrackView(int id, [FromBody] TrackViewRequest request, CancellationToken ct)
        {
            var userId = request.UserId ?? "anonymous";
            await _mediator.Send(new TrackViewCommand(id, userId), ct);
            return Ok();
        }

        /// <summary>Annonces de l'utilisateur connecté.</summary>
        [HttpGet("mine")]
        [Authorize]
        public async Task<IActionResult> GetMyAnnonces(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? sortBy,
            CancellationToken ct)
        {
            var userId = _currentUser.UserId.ToString();
            var result = await _mediator.Send(new GetMyListingsQuery(userId, search, status, sortBy), ct);
            return Ok(result);
        }

        /// <summary>Toutes les annonces triées par date décroissante.</summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetListingsQuery(), ct);
            return Ok(result);
        }

        /// <summary>Crée une annonce publiée.</summary>
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Create([FromBody] CreateAnnonceRequest request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Title) || request.CategoryId == 0)
                return BadRequest(new { message = "Le titre et la catégorie sont obligatoires." });

            var category = await _db.Categories
                .Include(c => c.Menu)
                .FirstOrDefaultAsync(c => c.Id == request.CategoryId, ct);

            var catName  = category?.Name?.ToLower() ?? "";
            var menuName = category?.Menu?.Name?.ToLower() ?? "";

            var isImmobilier = menuName.Contains("immobilier") || catName.Contains("immobilier")
                            || catName.Contains("immobili")   || catName.Contains("coloc")
                            || catName.Contains("location")   || catName.Contains("bureau")
                            || catName.Contains("commerce");

            var isBureauCommerce = catName.Contains("bureau") || catName.Contains("commerce");

            var condition = request.Condition ?? "";
            var location  = request.Location  ?? "";

            if (isImmobilier && request.ExtraData != null)
            {
                if (string.IsNullOrEmpty(condition) && request.ExtraData.TryGetValue("condition", out var condObj))
                    condition = condObj?.ToString() ?? "";
                if (string.IsNullOrEmpty(location) && request.ExtraData.TryGetValue("address", out var addrObj))
                    location = addrObj?.ToString() ?? "";
            }

            var price = request.Price;
            if (price == 0 && request.ExtraData != null)
            {
                if (isBureauCommerce
                    && request.ExtraData.TryGetValue("salePrice", out var spObj)
                    && decimal.TryParse(spObj?.ToString(), out var sp) && sp > 0)
                    price = sp;
                else if (request.ExtraData.TryGetValue("monthlyRent", out var rentObj)
                    && decimal.TryParse(rentObj?.ToString(), out var rent) && rent > 0)
                    price = rent;
            }

            var annonce = new Annonce
            {
                Title       = request.Title.Trim(),
                Description = request.Description ?? "",
                Price       = price,
                CategoryId  = request.CategoryId,
                AdType      = request.AdType ?? "",
                Condition   = condition,
                Location    = location,
                Phone       = request.Phone ?? "",
                Email       = request.Email ?? "",
                HidePhone   = request.HidePhone,
                ExtraData   = request.ExtraData != null
                                ? System.Text.Json.JsonSerializer.Serialize(request.ExtraData,
                                    new System.Text.Json.JsonSerializerOptions { WriteIndented = false })
                                : "{}",
                Status      = "published",
                UserId      = _currentUser.UserId.ToString()
            };

            _db.Annonces.Add(annonce);
            await _db.SaveChangesAsync(ct);

            return Ok(new { id = annonce.Id, message = "Annonce déposée avec succès !" });
        }

        /// <summary>Enregistre un brouillon d'annonce.</summary>
        [HttpPost("draft")]
        [AllowAnonymous]
        public async Task<IActionResult> SaveDraft([FromBody] CreateDraftRequest request, CancellationToken ct)
        {
            var annonce = new Annonce
            {
                Title       = request.Title ?? "Brouillon",
                Description = request.Description ?? "",
                Price       = request.Price,
                CategoryId  = request.CategoryId > 0 ? request.CategoryId : 1,
                AdType      = request.AdType ?? "",
                Condition   = request.Condition ?? "",
                Location    = request.Location ?? "",
                Phone       = request.Phone ?? "",
                Email       = request.Email ?? "",
                HidePhone   = request.HidePhone,
                CurrentStep = request.CurrentStep,
                ExtraData   = request.ExtraData != null
                                ? System.Text.Json.JsonSerializer.Serialize(request.ExtraData,
                                    new System.Text.Json.JsonSerializerOptions { WriteIndented = false })
                                : "{}",
                Status      = "draft",
                UserId      = _currentUser.UserId.ToString()
            };

            _db.Annonces.Add(annonce);
            await _db.SaveChangesAsync(ct);

            return Ok(new { id = annonce.Id, message = "Brouillon enregistré." });
        }

        /// <summary>Suggère des catégories basées sur les annonces existantes correspondant à la query.</summary>
        [HttpGet("suggest-categories")]
        [AllowAnonymous]
        public async Task<IActionResult> SuggestCategories([FromQuery] string query, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
                return Ok(new List<object>());

            var matchingCategoryIds = await _db.Annonces
                .Where(a => a.Title.Contains(query) || a.Description.Contains(query))
                .Select(a => a.CategoryId)
                .Distinct()
                .ToListAsync(ct);

            var categories = await _db.Categories
                .Where(c => matchingCategoryIds.Contains(c.Id))
                .Include(c => c.Menu)
                .ToListAsync(ct);

            var results = categories.Select(c => new
            {
                categoryId   = c.Id,
                categoryName = c.Name,
                menuId       = c.MenuId,
                menuName     = c.Menu?.Name ?? "",
                menuIcon     = c.Menu?.Icon ?? "",
                slug         = c.Slug
            }).Take(6);

            return Ok(results);
        }
    }
}
