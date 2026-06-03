using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MenusController : ControllerBase
{
    private readonly AppDbContext _context;

    public MenusController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Menu>>> GetAll()
    {
        return await _context.Menus.OrderBy(m => m.DisplayOrder).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Menu>> GetById(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return NotFound();
        return menu;
    }

    [HttpPost]
    public async Task<ActionResult<Menu>> Create(Menu menu)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = menu.Id }, menu);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, Menu menu)
    {
        if (id != menu.Id) return BadRequest();
        _context.Entry(menu).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var menu = await _context.Menus.FindAsync(id);
        if (menu == null) return NotFound();
        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
