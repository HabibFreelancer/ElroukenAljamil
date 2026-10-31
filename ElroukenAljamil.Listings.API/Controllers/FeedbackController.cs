using ElroukenAljamil.Listings.Application.Features.Feedback;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedbackController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<FeedbackController> _logger;

    public FeedbackController(IMediator mediator, ILogger<FeedbackController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateFeedbackRequest dto, CancellationToken ct)
    {
        var id = await _mediator.Send(new CreateFeedbackCommand(dto), ct);
        _logger.LogInformation("Feedback {FeedbackId} créé — note {Rating}", id, dto.Rating);
        return Ok(new { id });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetAllFeedbacksQuery(), ct);
        return Ok(result);
    }

    [HttpGet("stats")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetFeedbackStatsQuery(), ct);
        return Ok(result);
    }
}
