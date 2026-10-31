using ElroukenAljamil.Listings.Application.Commands.CreateAdType;
using ElroukenAljamil.Listings.Application.Commands.DeleteAdType;
using ElroukenAljamil.Listings.Application.Commands.UpdateAdType;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetAdTypeById;
using ElroukenAljamil.Listings.Application.Queries.GetAdTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<AdTypesController> _logger;

        public AdTypesController(IMediator mediator, ILogger<AdTypesController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<AdTypeDto>>> GetAll(CancellationToken ct) =>
            Ok(await _mediator.Send(new GetAdTypesQuery(), ct));

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AdTypeDto>> GetById(int id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAdTypeByIdQuery(id), ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<AdTypeDto>> Create([FromBody] CreateAdTypeRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new CreateAdTypeCommand(request), ct);
            _logger.LogInformation("AdType {AdTypeId} créé — {Label}", result.Id, result.Label);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdTypeRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateAdTypeCommand(id, request), ct);
            if (!success) _logger.LogWarning("Mise à jour AdType {AdTypeId} — introuvable", id);
            else _logger.LogInformation("AdType {AdTypeId} mis à jour", id);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteAdTypeCommand(id), ct);
            if (!success) _logger.LogWarning("Suppression AdType {AdTypeId} — introuvable", id);
            else _logger.LogInformation("AdType {AdTypeId} supprimé", id);
            return success ? NoContent() : NotFound();
        }
    }
}
