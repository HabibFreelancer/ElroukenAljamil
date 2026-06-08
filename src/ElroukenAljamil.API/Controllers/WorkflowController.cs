using ElroukenAljamil.Application.DTOs;
using ElroukenAljamil.Application.Services;
using ElroukenAljamil.Domain.Entities;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowService _workflowService;
    private readonly AppDbContext _context;

    public WorkflowController(WorkflowService workflowService, AppDbContext context)
    {
        _workflowService = workflowService;
        _context = context;
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<WorkflowDto>> GetByCategoryId(int categoryId)
    {
        var workflow = await _workflowService.GetWorkflowByCategoryId(categoryId);
        if (workflow == null) return NotFound();
        return Ok(workflow);
    }

    [HttpGet]
    public async Task<ActionResult<List<object>>> GetAll()
    {
        var workflows = await _context.DepositWorkflows
            .Include(w => w.Category)
            .Include(w => w.Steps)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new
            {
                w.Id,
                w.CategoryId,
                categoryName = w.Category != null ? w.Category.Name : "",
                w.Name,
                w.Description,
                w.IsActive,
                w.CreatedAt,
                stepsCount = w.Steps.Count
            })
            .ToListAsync();
        return Ok(workflows);
    }

    [HttpPost]
    public async Task<ActionResult> Create([FromBody] CreateWorkflowRequest req)
    {
        var workflow = new DepositWorkflow
        {
            CategoryId = req.CategoryId,
            Name = req.Name,
            Description = req.Description,
            IsActive = req.IsActive
        };
        _context.DepositWorkflows.Add(workflow);
        await _context.SaveChangesAsync();
        return Ok(new { id = workflow.Id });
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, [FromBody] CreateWorkflowRequest req)
    {
        var workflow = await _context.DepositWorkflows.FindAsync(id);
        if (workflow == null) return NotFound();
        workflow.CategoryId = req.CategoryId;
        workflow.Name = req.Name;
        workflow.Description = req.Description;
        workflow.IsActive = req.IsActive;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var workflow = await _context.DepositWorkflows.FindAsync(id);
        if (workflow == null) return NotFound();
        _context.DepositWorkflows.Remove(workflow);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // ===== Steps =====
    [HttpGet("{workflowId}/steps")]
    public async Task<ActionResult> GetSteps(int workflowId)
    {
        var steps = await _context.WorkflowSteps
            .Where(s => s.WorkflowId == workflowId)
            .OrderBy(s => s.StepOrder)
            .Select(s => new { s.Id, s.StepOrder, s.Title, s.Subtitle, s.StepKey, s.IsRequired, s.IsActive, fieldsCount = s.Fields.Count })
            .ToListAsync();
        return Ok(steps);
    }

    [HttpPost("{workflowId}/steps")]
    public async Task<ActionResult> CreateStep(int workflowId, [FromBody] CreateStepRequest req)
    {
        var step = new WorkflowStep
        {
            WorkflowId = workflowId,
            StepOrder = req.StepOrder,
            Title = req.Title,
            Subtitle = req.Subtitle,
            StepKey = req.StepKey,
            IsRequired = req.IsRequired,
            IsActive = true
        };
        _context.WorkflowSteps.Add(step);
        await _context.SaveChangesAsync();
        return Ok(new { id = step.Id });
    }

    [HttpPut("steps/{stepId}")]
    public async Task<ActionResult> UpdateStep(int stepId, [FromBody] CreateStepRequest req)
    {
        var step = await _context.WorkflowSteps.FindAsync(stepId);
        if (step == null) return NotFound();
        step.StepOrder = req.StepOrder;
        step.Title = req.Title;
        step.Subtitle = req.Subtitle;
        step.StepKey = req.StepKey;
        step.IsRequired = req.IsRequired;
        step.IsActive = req.IsActive;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("steps/{stepId}")]
    public async Task<ActionResult> DeleteStep(int stepId)
    {
        var step = await _context.WorkflowSteps.FindAsync(stepId);
        if (step == null) return NotFound();
        _context.WorkflowSteps.Remove(step);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // ===== Fields =====
    [HttpGet("steps/{stepId}/fields")]
    public async Task<ActionResult> GetFields(int stepId)
    {
        var fields = await _context.StepFields
            .Where(f => f.StepId == stepId)
            .OrderBy(f => f.DisplayOrder)
            .ToListAsync();
        return Ok(fields);
    }

    [HttpPost("steps/{stepId}/fields")]
    public async Task<ActionResult> CreateField(int stepId, [FromBody] CreateFieldRequest req)
    {
        var field = new StepField
        {
            StepId = stepId,
            FieldKey = req.FieldKey,
            Label = req.Label,
            FieldType = req.FieldType,
            Placeholder = req.Placeholder,
            Options = req.Options,
            DefaultValue = req.DefaultValue,
            Suffix = req.Suffix,
            HelperText = req.HelperText,
            IsRequired = req.IsRequired,
            DisplayOrder = req.DisplayOrder,
            IsActive = true,
            MaxLength = req.MaxLength
        };
        _context.StepFields.Add(field);
        await _context.SaveChangesAsync();
        return Ok(new { id = field.Id });
    }

    [HttpPut("fields/{fieldId}")]
    public async Task<ActionResult> UpdateField(int fieldId, [FromBody] CreateFieldRequest req)
    {
        var field = await _context.StepFields.FindAsync(fieldId);
        if (field == null) return NotFound();
        field.FieldKey = req.FieldKey;
        field.Label = req.Label;
        field.FieldType = req.FieldType;
        field.Placeholder = req.Placeholder;
        field.Options = req.Options;
        field.DefaultValue = req.DefaultValue;
        field.Suffix = req.Suffix;
        field.HelperText = req.HelperText;
        field.IsRequired = req.IsRequired;
        field.DisplayOrder = req.DisplayOrder;
        field.IsActive = req.IsActive;
        field.MaxLength = req.MaxLength;
        await _context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("fields/{fieldId}")]
    public async Task<ActionResult> DeleteField(int fieldId)
    {
        var field = await _context.StepFields.FindAsync(fieldId);
        if (field == null) return NotFound();
        _context.StepFields.Remove(field);
        await _context.SaveChangesAsync();
        return Ok();
    }
}

public class CreateWorkflowRequest
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
}

public class CreateStepRequest
{
    public int StepOrder { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Subtitle { get; set; } = string.Empty;
    public string StepKey { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public bool IsActive { get; set; } = true;
}

public class CreateFieldRequest
{
    public string FieldKey { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string FieldType { get; set; } = string.Empty;
    public string Placeholder { get; set; } = string.Empty;
    public string Options { get; set; } = string.Empty;
    public string DefaultValue { get; set; } = string.Empty;
    public string Suffix { get; set; } = string.Empty;
    public string HelperText { get; set; } = string.Empty;
    public bool IsRequired { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int? MaxLength { get; set; }
}
