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

        var annonce = new Annonce
        {
            Title = dto.Title,
            Description = dto.Description,
            Price = dto.Price,
            CategoryId = dto.CategoryId,
            AdType = dto.AdType,
            Condition = dto.Condition,
            Location = dto.Location,
            Phone = dto.Phone,
            Email = dto.Email,
            HidePhone = dto.HidePhone,
            ExtraData = dto.ExtraData != null ? JsonSerializer.Serialize(dto.ExtraData) : "",
            CreatedAt = DateTime.UtcNow
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
