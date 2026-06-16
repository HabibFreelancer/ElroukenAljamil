using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly AppDbContext _context;

    public FeedbackController(AppDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateFeedbackDto dto)
    {
        var feedback = new Feedback
        {
            AnnonceId = dto.AnnonceId,
            UserId = dto.UserId ?? "mock-user-1",
            UserEmail = dto.UserEmail ?? "habib.benradhouene@gmail.com",
            Rating = dto.Rating,
            Category = dto.Category ?? "",
            CreatedAt = DateTime.UtcNow
        };

        _context.Feedbacks.Add(feedback);
        await _context.SaveChangesAsync();

        return Ok(new { id = feedback.Id });
    }

    [HttpGet]
    public async Task<ActionResult> GetAll()
    {
        var feedbacks = await _context.Feedbacks
            .OrderByDescending(f => f.CreatedAt)
            .ToListAsync();
        return Ok(feedbacks);
    }

    [HttpGet("stats")]
    public async Task<ActionResult> GetStats()
    {
        var total = await _context.Feedbacks.CountAsync();
        var byRating = await _context.Feedbacks
            .GroupBy(f => f.Rating)
            .Select(g => new { rating = g.Key, count = g.Count() })
            .ToListAsync();
        return Ok(new { total, byRating });
    }
}

public class CreateFeedbackDto
{
    public int? AnnonceId { get; set; }
    public string? UserId { get; set; }
    public string? UserEmail { get; set; }
    public string Rating { get; set; } = "";
    public string? Category { get; set; }
}
