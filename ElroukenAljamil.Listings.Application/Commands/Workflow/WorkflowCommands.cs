using ElroukenAljamil.Listings.Application.DTOs;
using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.Workflow
{
    // ── Workflow CRUD ────────────────────────────────────────────────────────
    public record CreateWorkflowCommand(CreateWorkflowRequest Request) : IRequest<int>;
    public class CreateWorkflowCommandHandler : IRequestHandler<CreateWorkflowCommand, int>
    {
        private readonly IDepositWorkflowRepository _repo;
        public CreateWorkflowCommandHandler(IDepositWorkflowRepository repo) => _repo = repo;
        public async Task<int> Handle(CreateWorkflowCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var workflow = DepositWorkflow.Create(r.CategoryId, r.Name, r.Description, r.IsActive);
            await _repo.AddAsync(workflow, ct);
            return workflow.Id;
        }
    }

    public record UpdateWorkflowCommand(int Id, UpdateWorkflowRequest Request) : IRequest<bool>;
    public class UpdateWorkflowCommandHandler : IRequestHandler<UpdateWorkflowCommand, bool>
    {
        private readonly IDepositWorkflowRepository _repo;
        public UpdateWorkflowCommandHandler(IDepositWorkflowRepository repo) => _repo = repo;
        public async Task<bool> Handle(UpdateWorkflowCommand request, CancellationToken ct)
        {
            var workflow = await _repo.GetByIdAsync(request.Id, ct);
            if (workflow is null) return false;
            var r = request.Request;
            workflow.Update(r.CategoryId, r.Name, r.Description, r.IsActive);
            await _repo.UpdateAsync(workflow, ct);
            return true;
        }
    }

    public record DeleteWorkflowCommand(int Id) : IRequest<bool>;
    public class DeleteWorkflowCommandHandler : IRequestHandler<DeleteWorkflowCommand, bool>
    {
        private readonly IDepositWorkflowRepository _repo;
        public DeleteWorkflowCommandHandler(IDepositWorkflowRepository repo) => _repo = repo;
        public async Task<bool> Handle(DeleteWorkflowCommand request, CancellationToken ct)
        {
            var workflow = await _repo.GetByIdAsync(request.Id, ct);
            if (workflow is null) return false;
            await _repo.DeleteAsync(workflow, ct);
            return true;
        }
    }

    // ── Step CRUD ────────────────────────────────────────────────────────────
    public record CreateStepCommand(int WorkflowId, CreateStepRequest Request) : IRequest<int>;
    public class CreateStepCommandHandler : IRequestHandler<CreateStepCommand, int>
    {
        private readonly IWorkflowStepRepository _repo;
        public CreateStepCommandHandler(IWorkflowStepRepository repo) => _repo = repo;
        public async Task<int> Handle(CreateStepCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var step = WorkflowStep.Create(request.WorkflowId, r.StepOrder, r.Title, r.Subtitle, r.StepKey, r.IsRequired);
            await _repo.AddAsync(step, ct);
            return step.Id;
        }
    }

    public record UpdateStepCommand(int StepId, UpdateStepRequest Request) : IRequest<bool>;
    public class UpdateStepCommandHandler : IRequestHandler<UpdateStepCommand, bool>
    {
        private readonly IWorkflowStepRepository _repo;
        public UpdateStepCommandHandler(IWorkflowStepRepository repo) => _repo = repo;
        public async Task<bool> Handle(UpdateStepCommand request, CancellationToken ct)
        {
            var step = await _repo.GetByIdAsync(request.StepId, ct);
            if (step is null) return false;
            var r = request.Request;
            step.Update(r.StepOrder, r.Title, r.Subtitle, r.StepKey, r.IsRequired, r.IsActive);
            await _repo.UpdateAsync(step, ct);
            return true;
        }
    }

    public record DeleteStepCommand(int StepId) : IRequest<bool>;
    public class DeleteStepCommandHandler : IRequestHandler<DeleteStepCommand, bool>
    {
        private readonly IWorkflowStepRepository _repo;
        public DeleteStepCommandHandler(IWorkflowStepRepository repo) => _repo = repo;
        public async Task<bool> Handle(DeleteStepCommand request, CancellationToken ct)
        {
            var step = await _repo.GetByIdAsync(request.StepId, ct);
            if (step is null) return false;
            await _repo.DeleteAsync(step, ct);
            return true;
        }
    }

    // ── Field CRUD ───────────────────────────────────────────────────────────
    public record CreateFieldCommand(int StepId, CreateFieldRequest Request) : IRequest<int>;
    public class CreateFieldCommandHandler : IRequestHandler<CreateFieldCommand, int>
    {
        private readonly IStepFieldRepository _repo;
        public CreateFieldCommandHandler(IStepFieldRepository repo) => _repo = repo;
        public async Task<int> Handle(CreateFieldCommand request, CancellationToken ct)
        {
            var r = request.Request;
            var field = StepField.Create(request.StepId, r.FieldKey, r.Label, r.FieldType,
                r.Placeholder, r.Options, r.DefaultValue, r.Suffix, r.HelperText,
                r.IsRequired, r.DisplayOrder, r.MaxLength, r.ValidationRegex, r.VisibilityCondition);
            await _repo.AddAsync(field, ct);
            return field.Id;
        }
    }

    public record UpdateFieldCommand(int FieldId, UpdateFieldRequest Request) : IRequest<bool>;
    public class UpdateFieldCommandHandler : IRequestHandler<UpdateFieldCommand, bool>
    {
        private readonly IStepFieldRepository _repo;
        public UpdateFieldCommandHandler(IStepFieldRepository repo) => _repo = repo;
        public async Task<bool> Handle(UpdateFieldCommand request, CancellationToken ct)
        {
            var field = await _repo.GetByIdAsync(request.FieldId, ct);
            if (field is null) return false;
            var r = request.Request;
            field.Update(r.FieldKey, r.Label, r.FieldType, r.Placeholder, r.Options,
                r.DefaultValue, r.Suffix, r.HelperText, r.IsRequired, r.DisplayOrder,
                r.IsActive, r.MaxLength, r.ValidationRegex, r.VisibilityCondition);
            await _repo.UpdateAsync(field, ct);
            return true;
        }
    }

    public record DeleteFieldCommand(int FieldId) : IRequest<bool>;
    public class DeleteFieldCommandHandler : IRequestHandler<DeleteFieldCommand, bool>
    {
        private readonly IStepFieldRepository _repo;
        public DeleteFieldCommandHandler(IStepFieldRepository repo) => _repo = repo;
        public async Task<bool> Handle(DeleteFieldCommand request, CancellationToken ct)
        {
            var field = await _repo.GetByIdAsync(request.FieldId, ct);
            if (field is null) return false;
            await _repo.DeleteAsync(field, ct);
            return true;
        }
    }
}
