using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AdTypesController : ControllerBase
{
    private readonly AppDbContext _context;

    public AdTypesController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AdType>>> GetAll()
    {
        return await _context.AdTypes.Include(a => a.Category).OrderBy(a => a.CategoryId).ThenBy(a => a.DisplayOrder).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AdType>> GetById(int id)
    {
        var adType = await _context.AdTypes.FindAsync(id);
        if (adType == null) return NotFound();
        return adType;
    }

    [HttpPost]
    public async Task<ActionResult<AdType>> Create([FromBody] AdType adType)
    {
        adType.Category = null;
        _context.AdTypes.Add(adType);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetById), new { id = adType.Id }, adType);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] AdType adType)
    {
        if (id != adType.Id) return BadRequest();
        adType.Category = null;
        _context.Entry(adType).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var adType = await _context.AdTypes.FindAsync(id);
        if (adType == null) return NotFound();
        _context.AdTypes.Remove(adType);
        await _context.SaveChangesAsync();
        return NoContent();
    }
}
