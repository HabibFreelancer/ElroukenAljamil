using ElroukenAljamil.Application.DTOs;
using ElroukenAljamil.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly WorkflowService _workflowService;

    public WorkflowController(WorkflowService workflowService)
    {
        _workflowService = workflowService;
    }

    [HttpGet("{categoryId}")]
    public async Task<ActionResult<WorkflowDto>> GetByCategoryId(int categoryId)
    {
        var workflow = await _workflowService.GetWorkflowByCategoryId(categoryId);
        if (workflow == null) return NotFound();
        return Ok(workflow);
    }

    [HttpGet]
    public async Task<ActionResult<List<WorkflowDto>>> GetAll()
    {
        var workflows = await _workflowService.GetAllWorkflows();
        return Ok(workflows);
    }
}
