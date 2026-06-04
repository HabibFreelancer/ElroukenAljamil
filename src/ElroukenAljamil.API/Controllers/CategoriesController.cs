using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly AppDbContext _context;

    public CategoriesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Category>>> GetAll()
    {
        return await _context.Categories
            .Include(c => c.Menu)
            .OrderBy(c => c.MenuId)
            .ThenBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Category>> GetById(int id)
    {
        var category = await _context.Categories.Include(c => c.Menu).FirstOrDefaultAsync(c => c.Id == id);
        if (category == null) return NotFound();
        return category;
    }

    [HttpGet("by-menu/{menuId}")]
    public async Task<ActionResult<IEnumerable<Category>>> GetByMenu(int menuId)
    {
        return await _context.Categories
            .Where(c => c.MenuId == menuId && c.IsActive)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("for-deposit/{menuId}")]
    public async Task<ActionResult<IEnumerable<Category>>> GetForDeposit(int menuId)
    {
        return await _context.Categories
            .Where(c => c.MenuId == menuId && c.IsActive && c.ShowInDeposit && c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();
    }

    [HttpGet("tree/{menuId}")]
    public async Task<ActionResult<IEnumerable<Category>>> GetTree(int menuId)
    {
        var categories = await _context.Categories
            .Where(c => c.MenuId == menuId && c.IsActive && c.ParentCategoryId == null)
            .OrderBy(c => c.DisplayOrder)
            .ToListAsync();

        foreach (var cat in categories)
        {
            cat.SubCategories = await _context.Categories
                .Where(c => c.ParentCategoryId == cat.Id && c.IsActive)
                .OrderBy(c => c.DisplayOrder)
                .ToListAsync();
        }

        return categories;
    }

    [HttpPost]
    public async Task<ActionResult<Category>> Create([FromBody] Category category)
    {
        category.Menu = null!;
        category.ParentCategory = null;
        category.SubCategories = new List<Category>();
        _context.Categories.Add(category);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] Category category)
    {
        if (id != category.Id) return BadRequest();
        category.Menu = null!;
        category.ParentCategory = null;
        category.SubCategories = new List<Category>();
        _context.Entry(category).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return NotFound();
        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
