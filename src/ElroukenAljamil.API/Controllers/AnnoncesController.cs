using System.Text.Json;
using ElroukenAljamil.Application.DTOs;
using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AnnoncesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AnnoncesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult<object>> Create([FromBody] CreateAnnonceDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Title) || dto.CategoryId == 0)
            return BadRequest(new { message = "Le titre et la catégorie sont obligatoires." });

        // Detect immobilier category to enrich standard fields from ExtraData
        var category = await _context.Categories
            .Include(c => c.Menu)
            .FirstOrDefaultAsync(c => c.Id == dto.CategoryId);

        var isImmobilier = category?.Menu?.Name.ToLower().Contains("immobilier") == true
                        || category?.Name.ToLower().Contains("immobilier") == true
                        || category?.Name.ToLower().Contains("immobili") == true
                        || category?.Name.ToLower().Contains("coloc") == true
                        || category?.Name.ToLower().Contains("location") == true
                        || category?.Name.ToLower().Contains("bureau") == true
                        || category?.Name.ToLower().Contains("commerce") == true;

        var isBureauCommerce = category?.Name.ToLower().Contains("bureau") == true
                            || category?.Name.ToLower().Contains("commerce") == true;

        // Resolve condition: prefer ExtraData['condition'] for immobilier
        var condition = dto.Condition ?? "";
        var location  = dto.Location ?? "";

        if (isImmobilier && dto.ExtraData != null)
        {
            if (string.IsNullOrEmpty(condition) && dto.ExtraData.TryGetValue("condition", out var condObj))
                condition = condObj?.ToString() ?? "";
            if (string.IsNullOrEmpty(location) && dto.ExtraData.TryGetValue("address", out var addrObj))
                location = addrObj?.ToString() ?? "";
        }

        // Resolve price:
        // - colocation + location → monthlyRent
        // - bureau/commerce       → salePrice
        // - others                → price field
        var price = dto.Price;
        if (price == 0 && dto.ExtraData != null)
        {
            if (isBureauCommerce
                && dto.ExtraData.TryGetValue("salePrice", out var spObj)
                && decimal.TryParse(spObj?.ToString(), out var sp) && sp > 0)
            {
                price = sp;
            }
            else if (dto.ExtraData.TryGetValue("monthlyRent", out var rentObj)
                && decimal.TryParse(rentObj?.ToString(), out var rent) && rent > 0)
            {
                price = rent;
            }
        }

        var annonce = new Annonce
        {
            Title       = dto.Title.Trim(),
            Description = dto.Description ?? "",
            Price       = price,
            CategoryId  = dto.CategoryId,
            AdType      = dto.AdType ?? "",
            Condition   = condition,
            Location    = location,
            Phone       = dto.Phone ?? "",
            Email       = dto.Email ?? "",
            HidePhone   = dto.HidePhone,
            ExtraData   = dto.ExtraData != null
                            ? JsonSerializer.Serialize(dto.ExtraData,
                                new JsonSerializerOptions { WriteIndented = false })
                            : "{}",
            CreatedAt   = DateTime.UtcNow,
            Status      = "published"
        };

        _context.Annonces.Add(annonce);
        await _context.SaveChangesAsync();

        return Ok(new { id = annonce.Id, message = "Annonce déposée avec succès !" });
    }

    [HttpPost("draft")]
    public async Task<ActionResult<object>> SaveDraft([FromBody] CreateDraftDto dto)
    {
        var annonce = new Annonce
        {
            Title = dto.Title ?? "Brouillon",
            Description = dto.Description ?? "",
            Price = dto.Price,
            CategoryId = dto.CategoryId > 0 ? dto.CategoryId : 1,
            AdType = dto.AdType ?? "",
            Condition = dto.Condition ?? "",
            Location = dto.Location ?? "",
            Phone = dto.Phone ?? "",
            Email = dto.Email ?? "",
            HidePhone = dto.HidePhone,
            Status = "draft",
            CurrentStep = dto.CurrentStep,
            ExtraData = dto.ExtraData != null ? JsonSerializer.Serialize(dto.ExtraData) : "",
            CreatedAt = DateTime.UtcNow
        };

        _context.Annonces.Add(annonce);
        await _context.SaveChangesAsync();

        return Ok(new { id = annonce.Id, message = "Brouillon enregistr\u00e9." });
    }

    [HttpGet("suggest-categories")]
    public async Task<ActionResult<IEnumerable<object>>> SuggestCategories([FromQuery] string query)
    {
        if (string.IsNullOrWhiteSpace(query) || query.Length < 2)
            return Ok(new List<object>());

        // Chercher les annonces qui contiennent le mot et récupérer leurs catégories
        var matchingCategoryIds = await _context.Annonces
            .Where(a => a.Title.Contains(query) || a.Description.Contains(query))
            .Select(a => a.CategoryId)
            .Distinct()
            .ToListAsync();

        // Récupérer les catégories avec leur menu
        var categories = await _context.Categories
            .Where(c => matchingCategoryIds.Contains(c.Id))
            .Include(c => c.Menu)
            .ToListAsync();

        var results = categories.Select(c => new
        {
            categoryId = c.Id,
            categoryName = c.Name,
            menuId = c.MenuId,
            menuName = c.Menu?.Name ?? "",
            menuIcon = c.Menu?.Icon ?? "",
            slug = c.Slug
        }).Take(6).ToList();

        return Ok(results);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Annonce>>> GetAll()
    {
        return await _context.Annonces.OrderByDescending(a => a.CreatedAt).ToListAsync();
    }

    [HttpGet("my")]
    public async Task<ActionResult> GetMyAnnonces([FromQuery] string? email, [FromQuery] string? search, [FromQuery] string? status, [FromQuery] string? sortBy)
    {
        var query = _context.Annonces.AsQueryable();

        if (!string.IsNullOrEmpty(email))
            query = query.Where(a => a.Email == email);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(a => a.Title.Contains(search) || a.Description.Contains(search));

        if (!string.IsNullOrEmpty(status))
            query = query.Where(a => a.Status == status);
        else
            query = query.Where(a => a.Status != "draft");

        // Sorting
        query = sortBy switch
        {
            "price_asc" => query.OrderBy(a => a.Price),
            "price_desc" => query.OrderByDescending(a => a.Price),
            _ => query.OrderByDescending(a => a.CreatedAt)
        };

        var annonces = await query.ToListAsync();

        var result = annonces.Select(a => new
        {
            a.Id, a.Title, a.Description, a.Price, a.CategoryId, a.AdType,
            a.Location, a.Status, a.CreatedAt,
            category = _context.Categories.Where(c => c.Id == a.CategoryId).Select(c => c.Name).FirstOrDefault() ?? "",
            views = _context.AnnonceViews.Count(v => v.AnnonceId == a.Id),
            favorites = _context.AnnonceFavorites.Count(f => f.AnnonceId == a.Id),
            messages = _context.Messages.Count(m => m.AnnonceId == a.Id)
        });

        return Ok(result);
    }

    [HttpPost("{id}/view")]
    public async Task<ActionResult> TrackView(int id, [FromBody] TrackRequest req)
    {
        _context.AnnonceViews.Add(new AnnonceView { AnnonceId = id, UserId = req.UserId ?? "anonymous" });
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpPost("{id}/favorite")]
    public async Task<ActionResult> ToggleFavorite(int id, [FromBody] TrackRequest req)
    {
        var userId = req.UserId ?? "anonymous";
        var existing = await _context.AnnonceFavorites.FirstOrDefaultAsync(f => f.AnnonceId == id && f.UserId == userId);
        if (existing != null)
        {
            _context.AnnonceFavorites.Remove(existing);
            await _context.SaveChangesAsync();
            return Ok(new { favorited = false });
        }
        _context.AnnonceFavorites.Add(new AnnonceFavorite { AnnonceId = id, UserId = userId });
        await _context.SaveChangesAsync();
        return Ok(new { favorited = true });
    }

    [HttpPost("{id}/message")]
    public async Task<ActionResult> SendMessage(int id, [FromBody] SendMessageRequest req)
    {
        var annonce = await _context.Annonces.FindAsync(id);
        if (annonce == null) return NotFound();

        var message = new Message
        {
            AnnonceId = id,
            SenderId = req.SenderId ?? "anonymous",
            SenderEmail = req.SenderEmail ?? "",
            ReceiverId = annonce.Email,
            Content = req.Content ?? ""
        };
        _context.Messages.Add(message);
        await _context.SaveChangesAsync();
        return Ok(new { id = message.Id });
    }

    [HttpGet("{id}/messages")]
    public async Task<ActionResult> GetMessages(int id)
    {
        var messages = await _context.Messages
            .Where(m => m.AnnonceId == id)
            .OrderByDescending(m => m.CreatedAt)
            .ToListAsync();
        return Ok(messages);
    }

    [HttpPut("{id}/pause")]
    public async Task<ActionResult> PauseAnnonce(int id)
    {
        var annonce = await _context.Annonces.FindAsync(id);
        if (annonce == null) return NotFound();
        annonce.Status = annonce.Status == "paused" ? "published" : "paused";
        await _context.SaveChangesAsync();
        return Ok(new { status = annonce.Status });
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteAnnonce(int id)
    {
        var annonce = await _context.Annonces.FindAsync(id);
        if (annonce == null) return NotFound();
        _context.Annonces.Remove(annonce);
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetById(int id)
    {
        var annonce = await _context.Annonces.FindAsync(id);
        if (annonce == null) return NotFound();

        var category = await _context.Categories
            .Include(c => c.Menu)
            .FirstOrDefaultAsync(c => c.Id == annonce.CategoryId);

        var categoryName = category?.Name ?? "";
        var menuName     = category?.Menu?.Name ?? "";
        var views        = await _context.AnnonceViews.CountAsync(v => v.AnnonceId == id);
        var favorites    = await _context.AnnonceFavorites.CountAsync(f => f.AnnonceId == id);

        // Deserialize ExtraData
        Dictionary<string, JsonElement>? extra = null;
        if (!string.IsNullOrEmpty(annonce.ExtraData))
        {
            try
            {
                extra = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(annonce.ExtraData);
            }
            catch { /* keep null */ }
        }

        var isImmobilier = menuName.ToLower().Contains("immobilier")
                        || categoryName.ToLower().Contains("immobilier")
                        || categoryName.ToLower().Contains("immobili")
                        || categoryName.ToLower().Contains("coloc")
                        || categoryName.ToLower().Contains("location");

        var isColocation = categoryName.ToLower().Contains("coloc");

        // Build immobilier/colocation-specific fields from ExtraData
        object? immobilierDetails = null;
        if (isImmobilier && extra != null)
        {
            immobilierDetails = new
            {
                // Common immobilier fields
                propertyType     = GetExtraString(extra, "propertyType"),
                surface          = GetExtraString(extra, "surface"),
                rooms            = GetExtraString(extra, "rooms"),
                bedrooms         = GetExtraString(extra, "bedrooms"),
                bathrooms        = GetExtraString(extra, "bathrooms"),
                cuisine          = GetExtraString(extra, "cuisine"),
                levels           = GetExtraString(extra, "levels"),
                floor            = GetExtraString(extra, "floor"),
                totalFloors      = GetExtraString(extra, "totalFloors"),
                elevator         = GetExtraString(extra, "elevator"),
                constructionYear = GetExtraString(extra, "constructionYear"),
                condition        = GetExtraString(extra, "condition"),
                propertyNature   = GetExtraString(extra, "propertyNature"),
                terrainNature    = GetExtraString(extra, "terrainNature"),
                parkingNature    = GetExtraString(extra, "parkingNature"),
                features         = GetExtraString(extra, "features"),
                landSurface      = GetExtraString(extra, "landSurface"),
                parkingSpots     = GetExtraString(extra, "parking"),
                heatingMode      = GetExtraString(extra, "heatingMode"),
                exterior         = GetExtraString(extra, "exterior"),
                exposure         = GetExtraString(extra, "exposure"),
                availableFrom    = GetExtraString(extra, "availableFrom"),
                // Location / Colocation fields
                furnished        = GetExtraString(extra, "furnished"),
                monthlyRent      = GetExtraString(extra, "monthlyRent"),
                deposit          = GetExtraString(extra, "deposit"),
                // Colocation-specific fields
                roomType         = GetExtraString(extra, "roomType"),
                roommatesCount   = GetExtraString(extra, "roommatesCount"),
                smokingPolicy    = GetExtraString(extra, "smokingPolicy"),
                petsAllowed      = GetExtraString(extra, "petsAllowed"),
                heatingType      = GetExtraString(extra, "heatingType"),
            };
        }

        return Ok(new
        {
            annonce.Id,
            annonce.Title,
            annonce.Description,
            annonce.Price,
            annonce.CategoryId,
            annonce.AdType,
            annonce.Condition,
            annonce.Location,
            annonce.Phone,
            annonce.Email,
            annonce.HidePhone,
            annonce.ExtraData,
            annonce.Status,
            annonce.CreatedAt,
            category        = categoryName,
            menu            = menuName,
            isImmobilier,
            isColocation,
            immobilierDetails,
            views,
            favorites
        });
    }

    private static string GetExtraString(Dictionary<string, JsonElement> extra, string key)
    {
        if (!extra.TryGetValue(key, out var el)) return "";
        return el.ValueKind switch
        {
            JsonValueKind.String => el.GetString() ?? "",
            JsonValueKind.Number => el.GetRawText(),
            JsonValueKind.True   => "true",
            JsonValueKind.False  => "false",
            JsonValueKind.Array  => string.Join(", ", el.EnumerateArray().Select(e => e.GetString() ?? e.GetRawText())),
            _                    => el.GetRawText()
        };
    }

    [HttpGet("ad-types/{categoryId}")]
    public async Task<ActionResult<IEnumerable<AdType>>> GetAdTypes(int categoryId)
    {
        var adTypes = await _context.AdTypes
            .Where(a => a.CategoryId == categoryId && a.IsActive)
            .OrderBy(a => a.DisplayOrder)
            .ToListAsync();

        // Si pas de résultat, chercher aussi par le parent
        if (!adTypes.Any())
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category?.ParentCategoryId != null)
            {
                adTypes = await _context.AdTypes
                    .Where(a => a.CategoryId == category.ParentCategoryId && a.IsActive)
                    .OrderBy(a => a.DisplayOrder)
                    .ToListAsync();
            }
        }

        return adTypes;
    }

    [HttpPost("price-estimate")]
    public async Task<ActionResult> GetPriceEstimate([FromBody] PriceEstimateRequest req)
    {
        // Find similar annonces by brand+model or category
        var query = _context.Annonces.Where(a => a.CategoryId == req.CategoryId && a.Price > 0);

        // Filter by brand/model if available via ExtraData
        var allAds = await query.OrderByDescending(a => a.CreatedAt).Take(100).ToListAsync();

        var similar = allAds.Where(a =>
        {
            if (string.IsNullOrEmpty(req.Brand)) return true;
            return a.ExtraData.Contains(req.Brand, StringComparison.OrdinalIgnoreCase);
        }).Take(10).ToList();

        // If not enough with brand filter, use all
        if (similar.Count < 3) similar = allAds.Take(10).ToList();

        var prices = similar.Where(a => a.Price > 0).Select(a => a.Price).ToList();

        // Price gauge calculation
        decimal minPrice = 0, maxPrice = 0, avgPrice = 0;
        if (prices.Any())
        {
            minPrice = prices.Min();
            maxPrice = prices.Max();
            avgPrice = prices.Average();
        }

        // Similar ads (top 5)
        var similarAds = similar.Take(5).Select(a => new
        {
            id = a.Id,
            title = a.Title,
            price = a.Price,
            mileage = ExtractMileage(a.ExtraData),
            location = a.Location,
            image = $"https://placehold.co/200x130/f0f0f0/666?text={Uri.EscapeDataString(ExtractBrandModel(a.ExtraData))}"
        }).ToList();

        return Ok(new
        {
            minPrice,
            maxPrice,
            avgPrice,
            count = prices.Count,
            similarAds
        });
    }

    private string ExtractMileage(string extraData)
    {
        if (string.IsNullOrEmpty(extraData)) return "";
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(extraData);
            if (data.TryGetProperty("mileage", out var m)) return m.ToString();
        }
        catch { }
        return "";
    }

    private string ExtractBrandModel(string extraData)
    {
        if (string.IsNullOrEmpty(extraData)) return "Auto";
        try
        {
            var data = JsonSerializer.Deserialize<JsonElement>(extraData);
            var brand = data.TryGetProperty("brand", out var b) ? b.ToString() : "";
            var model = data.TryGetProperty("model", out var m) ? m.ToString() : "";
            var result = $"{brand} {model}".Trim();
            return string.IsNullOrEmpty(result) ? "Auto" : result;
        }
        catch { return "Auto"; }
    }
}

public class PriceEstimateRequest
{
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}

public class CreateDraftDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public int CategoryId { get; set; }
    public string? AdType { get; set; }
    public string? Condition { get; set; }
    public string? Location { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool HidePhone { get; set; }
    public int CurrentStep { get; set; }
    public Dictionary<string, object>? ExtraData { get; set; }
}

public class TrackRequest
{
    public string? UserId { get; set; }
}

public class SendMessageRequest
{
    public string? SenderId { get; set; }
    public string? SenderEmail { get; set; }
    public string? Content { get; set; }
}
