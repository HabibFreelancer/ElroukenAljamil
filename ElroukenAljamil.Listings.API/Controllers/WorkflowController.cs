using ElroukenAljamil.Listings.Application.Commands.Workflow;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Application.Queries.GetWorkflow;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WorkflowController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<WorkflowController> _logger;

        public WorkflowController(IMediator mediator, ILogger<WorkflowController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

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
            _logger.LogInformation("Workflow {WorkflowId} créé", id);
            return Ok(new { id });
        }

        [HttpPut("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWorkflowRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateWorkflowCommand(id, request), ct);
            if (!success) _logger.LogWarning("Mise à jour workflow {WorkflowId} — introuvable", id);
            else _logger.LogInformation("Workflow {WorkflowId} mis à jour", id);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("{id:int}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteWorkflowCommand(id), ct);
            if (!success) _logger.LogWarning("Suppression workflow {WorkflowId} — introuvable", id);
            else _logger.LogInformation("Workflow {WorkflowId} supprimé", id);
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
            _logger.LogInformation("Step {StepId} créé dans workflow {WorkflowId}", id, workflowId);
            return Ok(new { id });
        }

        [HttpPut("steps/{stepId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateStep(int stepId, [FromBody] UpdateStepRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateStepCommand(stepId, request), ct);
            if (!success) _logger.LogWarning("Mise à jour step {StepId} — introuvable", stepId);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("steps/{stepId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteStep(int stepId, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteStepCommand(stepId), ct);
            if (!success) _logger.LogWarning("Suppression step {StepId} — introuvable", stepId);
            else _logger.LogInformation("Step {StepId} supprimé", stepId);
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
            _logger.LogInformation("Field {FieldId} créé dans step {StepId}", id, stepId);
            return Ok(new { id });
        }

        [HttpPut("fields/{fieldId:int}")]
        [Authorize]
        public async Task<IActionResult> UpdateField(int fieldId, [FromBody] UpdateFieldRequest request, CancellationToken ct)
        {
            var success = await _mediator.Send(new UpdateFieldCommand(fieldId, request), ct);
            if (!success) _logger.LogWarning("Mise à jour field {FieldId} — introuvable", fieldId);
            return success ? Ok() : NotFound();
        }

        [HttpDelete("fields/{fieldId:int}")]
        [Authorize]
        public async Task<IActionResult> DeleteField(int fieldId, CancellationToken ct)
        {
            var success = await _mediator.Send(new DeleteFieldCommand(fieldId), ct);
            if (!success) _logger.LogWarning("Suppression field {FieldId} — introuvable", fieldId);
            else _logger.LogInformation("Field {FieldId} supprimé", fieldId);
            return success ? Ok() : NotFound();
        }
    }
}
