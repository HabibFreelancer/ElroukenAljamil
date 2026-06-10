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
            location = a.Location
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
}

public class PriceEstimateRequest
{
    public int CategoryId { get; set; }
    public string Brand { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
}
