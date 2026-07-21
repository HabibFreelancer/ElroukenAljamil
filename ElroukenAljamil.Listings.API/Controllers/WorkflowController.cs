using ElroukenAljamil.Listings.Application.Commands.Workflow;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetWorkflow;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;
        public WorkflowController(IMediator mediator) => _mediator = mediator;

        // ── Workflows ────────────────────────────────────────────────────────

        [HttpGet]
        public async Task<ActionResult<List<WorkflowDto>>> GetAll(CancellationToken ct) =>
            Ok(await _mediator.Send(new GetWorkflowsQuery(), ct));

        [HttpGet("{categoryId:int}")]
        public async Task<ActionResult<WorkflowDto>> GetByCategoryId(int categoryId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetWorkflowByCategoryQuery(categoryId), ct);
            return result is null ? NotFound() : Ok(result);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateWorkflowRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateWorkflowCommand(request), ct);
            return Ok(new { id });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkflowRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateWorkflowCommand(id, request), ct);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteWorkflowCommand(id), ct);
            return success ? Ok() : NotFound();
        }

        // ── Steps ────────────────────────────────────────────────────────────

        [HttpGet("{workflowId:int}/steps")]
        public async Task<ActionResult<List<WorkflowStepDto>>> GetSteps(int workflowId, CancellationToken ct) =>
            Ok(await _mediator.Send(new GetWorkflowStepsQuery(workflowId), ct));

        [HttpPost("{workflowId:int}/steps")]
        [Authorize]
        public async Task<IActionResult> CreateStep(int workflowId, [FromBody] CreateStepRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateStepCommand(workflowId, request), ct);
            return Ok(new { id });
        }

        [HttpPut("steps/{stepId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateStepCommand(stepId, request), ct);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("steps/{stepId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteStep(int stepId, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteStepCommand(stepId), ct);
            return success ? Ok() : NotFound();
        }

        // ── Fields ───────────────────────────────────────────────────────────

        [HttpGet("steps/{stepId:int}/fields")]
        public async Task<ActionResult<List<StepFieldDto>>> GetFields(int stepId, CancellationToken ct) =>
            Ok(await _mediator.Send(new GetStepFieldsQuery(stepId), ct));

        [HttpPost("steps/{stepId:int}/fields")]
        [Authorize]
        public async Task<IActionResult> CreateField(int stepId, [FromBody] CreateFieldRequest request, CancellationToken ct)
        {
            var id = await _mediator.Send(new CreateFieldCommand(stepId, request), ct);
            return Ok(new { id });
        }

        [HttpPut("fields/{fieldId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateField(int fieldId, [FromBody] UpdateFieldRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateFieldCommand(fieldId, request), ct);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("fields/{fieldId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteField(int fieldId, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteFieldCommand(fieldId), ct);
            return success ? Ok() : NotFound();
        }
    }
}
