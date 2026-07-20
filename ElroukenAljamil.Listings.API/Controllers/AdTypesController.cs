using ElroukenAljamil.Listings.Application.Commands.CreateAdType;
using ElroukenAljamil.Listings.Application.Commands.DeleteAdType;
using ElroukenAljamil.Listings.Application.Commands.UpdateAdType;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetAdTypeById;
using ElroukenAljamil.Listings.Application.Queries.GetAdTypes;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AdTypesController : ControllerBase
    {
        private readonly IMediator _mediator;
        public AdTypesController(IMediator mediator) => _mediator = mediator;

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
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateAdTypeRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateAdTypeCommand(id, request), ct);
            return success ? NoContent() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteAdTypeCommand(id), ct);
            return success ? NoContent() : NotFound();
        }
    }
}
