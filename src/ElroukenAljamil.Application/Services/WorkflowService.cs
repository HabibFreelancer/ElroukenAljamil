using System.Text.Json;
using ElroukenAljamil.Application.DTOs;
using ElroukenAljamil.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Application.Services;

public class WorkflowService
{
    private readonly AppDbContext _context;

    public WorkflowService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<WorkflowDto?> GetWorkflowByCategoryId(int categoryId)
    {
        var workflow = await _context.DepositWorkflows
            .Include(w => w.Steps.Where(s => s.IsActive))
                .ThenInclude(s => s.Fields.Where(f => f.IsActive))
            .Where(w => w.CategoryId == categoryId && w.IsActive)
            .FirstOrDefaultAsync();

        if (workflow == null)
        {
            // Try parent category
            var category = await _context.Categories.FindAsync(categoryId);
            if (category?.ParentCategoryId != null)
            {
                workflow = await _context.DepositWorkflows
                    .Include(w => w.Steps.Where(s => s.IsActive))
                        .ThenInclude(s => s.Fields.Where(f => f.IsActive))
                    .Where(w => w.CategoryId == category.ParentCategoryId && w.IsActive)
                    .FirstOrDefaultAsync();
            }
        }

        if (workflow == null) return null;

        return MapToDto(workflow);
    }

    public async Task<List<WorkflowDto>> GetAllWorkflows()
    {
        var workflows = await _context.DepositWorkflows
            .Include(w => w.Category)
            .Include(w => w.Steps.Where(s => s.IsActive))
                .ThenInclude(s => s.Fields.Where(f => f.IsActive))
            .Where(w => w.IsActive)
            .ToListAsync();

        return workflows.Select(MapToDto).ToList();
    }

    private WorkflowDto MapToDto(Domain.Entities.DepositWorkflow workflow)
    {
        return new WorkflowDto
        {
            Id = workflow.Id,
            CategoryId = workflow.CategoryId,
            Name = workflow.Name,
            Description = workflow.Description,
            Steps = workflow.Steps
                .OrderBy(s => s.StepOrder)
                .Select(s => new WorkflowStepDto
                {
                    Id = s.Id,
                    StepOrder = s.StepOrder,
                    Title = s.Title,
                    Subtitle = s.Subtitle,
                    StepKey = s.StepKey,
                    IsRequired = s.IsRequired,
                    Fields = s.Fields
                        .OrderBy(f => f.DisplayOrder)
                        .Select(f => new StepFieldDto
                        {
                            Id = f.Id,
                            FieldKey = f.FieldKey,
                            Label = f.Label,
                            FieldType = f.FieldType,
                            Placeholder = f.Placeholder,
                            Options = ParseOptions(f.Options),
                            DefaultValue = f.DefaultValue,
                            Suffix = f.Suffix,
                            HelperText = f.HelperText,
                            IsRequired = f.IsRequired,
                            DisplayOrder = f.DisplayOrder,
                            MaxLength = f.MaxLength
                        }).ToList()
                }).ToList()
        };
    }

    private List<FieldOptionDto> ParseOptions(string optionsJson)
    {
        if (string.IsNullOrWhiteSpace(optionsJson)) return new();
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<List<FieldOptionDto>>(optionsJson, options) ?? new();
        }
        catch
        {
            return new();
        }
    }
}
