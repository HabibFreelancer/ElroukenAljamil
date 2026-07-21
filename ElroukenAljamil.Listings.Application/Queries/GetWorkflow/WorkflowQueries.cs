using System.Text.Json;
using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GetWorkflow
{
    // ── GetAll ──────────────────────────────────────────────────────────────
    public record GetWorkflowsQuery : IRequest<List<WorkflowDto>>;

    public class GetWorkflowsQueryHandler : IRequestHandler<GetWorkflowsQuery, List<WorkflowDto>>
    {
        private readonly IDepositWorkflowRepository _repo;
        public GetWorkflowsQueryHandler(IDepositWorkflowRepository repo) => _repo = repo;

        public async Task<List<WorkflowDto>> Handle(GetWorkflowsQuery request, CancellationToken ct)
        {
            var workflows = await _repo.GetAllAsync(ct);
            return workflows.Select(w => new WorkflowDto
            {
                Id = w.Id,
                CategoryId = w.CategoryId,
                CategoryName = w.Category?.Name ?? string.Empty,
                Name = w.Name,
                Description = w.Description,
                IsActive = w.IsActive,
                CreatedAt = w.CreatedAt,
                StepsCount = w.Steps.Count
            }).ToList();
        }
    }

    // ── GetByCategoryId ─────────────────────────────────────────────────────
    public record GetWorkflowByCategoryQuery(int CategoryId) : IRequest<WorkflowDto?>;

    public class GetWorkflowByCategoryQueryHandler : IRequestHandler<GetWorkflowByCategoryQuery, WorkflowDto?>
    {
        private readonly IDepositWorkflowRepository _repo;
        public GetWorkflowByCategoryQueryHandler(IDepositWorkflowRepository repo) => _repo = repo;

        public async Task<WorkflowDto?> Handle(GetWorkflowByCategoryQuery request, CancellationToken ct)
        {
            var w = await _repo.GetByCategoryIdAsync(request.CategoryId, ct);
            return w is null ? null : MapFull(w);
        }

        private static WorkflowDto MapFull(DepositWorkflow w) => new()
        {
            Id = w.Id,
            CategoryId = w.CategoryId,
            CategoryName = w.Category?.Name ?? string.Empty,
            Name = w.Name,
            Description = w.Description,
            IsActive = w.IsActive,
            CreatedAt = w.CreatedAt,
            StepsCount = w.Steps.Count,
            Steps = w.Steps.OrderBy(s => s.StepOrder).Select(s => new WorkflowStepDto
            {
                Id = s.Id,
                StepOrder = s.StepOrder,
                Title = s.Title,
                Subtitle = s.Subtitle,
                StepKey = s.StepKey,
                IsRequired = s.IsRequired,
                IsActive = s.IsActive,
                FieldsCount = s.Fields.Count,
                Fields = s.Fields.OrderBy(f => f.DisplayOrder).Select(f => MapField(f)).ToList()
            }).ToList()
        };

        private static StepFieldDto MapField(StepField f) => new()
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
            IsActive = f.IsActive,
            MaxLength = f.MaxLength,
            ValidationRegex = f.ValidationRegex,
            VisibilityCondition = f.VisibilityCondition
        };

        private static List<FieldOptionDto> ParseOptions(string json)
        {
            if (string.IsNullOrWhiteSpace(json)) return new();
            try { return JsonSerializer.Deserialize<List<FieldOptionDto>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new(); }
            catch { return new(); }
        }
    }

    // ── GetSteps ────────────────────────────────────────────────────────────
    public record GetWorkflowStepsQuery(int WorkflowId) : IRequest<List<WorkflowStepDto>>;

    public class GetWorkflowStepsQueryHandler : IRequestHandler<GetWorkflowStepsQuery, List<WorkflowStepDto>>
    {
        private readonly IWorkflowStepRepository _repo;
        public GetWorkflowStepsQueryHandler(IWorkflowStepRepository repo) => _repo = repo;

        public async Task<List<WorkflowStepDto>> Handle(GetWorkflowStepsQuery request, CancellationToken ct)
        {
            var steps = await _repo.GetByWorkflowIdAsync(request.WorkflowId, ct);
            return steps.Select(s => new WorkflowStepDto
            {
                Id = s.Id,
                StepOrder = s.StepOrder,
                Title = s.Title,
                Subtitle = s.Subtitle,
                StepKey = s.StepKey,
                IsRequired = s.IsRequired,
                IsActive = s.IsActive,
                FieldsCount = s.Fields.Count
            }).ToList();
        }
    }

    // ── GetFields ───────────────────────────────────────────────────────────
    public record GetStepFieldsQuery(int StepId) : IRequest<List<StepFieldDto>>;

    public class GetStepFieldsQueryHandler : IRequestHandler<GetStepFieldsQuery, List<StepFieldDto>>
    {
        private readonly IStepFieldRepository _repo;
        public GetStepFieldsQueryHandler(IStepFieldRepository repo) => _repo = repo;

        public async Task<List<StepFieldDto>> Handle(GetStepFieldsQuery request, CancellationToken ct)
        {
            var fields = await _repo.GetByStepIdAsync(request.StepId, ct);
            return fields.Select(f => new StepFieldDto
            {
                Id = f.Id,
                FieldKey = f.FieldKey,
                Label = f.Label,
                FieldType = f.FieldType,
                Placeholder = f.Placeholder,
                DefaultValue = f.DefaultValue,
                Suffix = f.Suffix,
                HelperText = f.HelperText,
                IsRequired = f.IsRequired,
                DisplayOrder = f.DisplayOrder,
                IsActive = f.IsActive,
                MaxLength = f.MaxLength,
                ValidationRegex = f.ValidationRegex,
                VisibilityCondition = f.VisibilityCondition
            }).ToList();
        }
    }
}
