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
}
