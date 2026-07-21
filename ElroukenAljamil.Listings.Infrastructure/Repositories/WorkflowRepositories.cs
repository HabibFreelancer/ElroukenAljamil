using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using ElroukenAljamil.Listings.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Listings.Infrastructure.Repositories
{
    public class DepositWorkflowRepository : IDepositWorkflowRepository
    {
        private readonly ListingsDbContext _context;
        public DepositWorkflowRepository(ListingsDbContext context) => _context = context;

        public Task<List<DepositWorkflow>> GetAllAsync(CancellationToken ct = default) =>
            _context.DepositWorkflows
                .Include(w => w.Category)
                .Include(w => w.Steps)
                .OrderByDescending(w => w.CreatedAt)
                .ToListAsync(ct);

        public Task<DepositWorkflow?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.DepositWorkflows
                .Include(w => w.Category)
                .Include(w => w.Steps).ThenInclude(s => s.Fields)
                .FirstOrDefaultAsync(w => w.Id == id, ct);

        public async Task<DepositWorkflow?> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default)
        {
            var workflow = await _context.DepositWorkflows
                .Include(w => w.Steps.Where(s => s.IsActive)).ThenInclude(s => s.Fields.Where(f => f.IsActive))
                .FirstOrDefaultAsync(w => w.CategoryId == categoryId && w.IsActive, ct);

            if (workflow is not null) return workflow;

            // Fallback sur la catégorie parente
            var category = await _context.Categories.FindAsync(new object[] { categoryId }, ct);
            if (category?.ParentCategoryId is null) return null;

            return await _context.DepositWorkflows
                .Include(w => w.Steps.Where(s => s.IsActive)).ThenInclude(s => s.Fields.Where(f => f.IsActive))
                .FirstOrDefaultAsync(w => w.CategoryId == category.ParentCategoryId && w.IsActive, ct);
        }

        public async Task AddAsync(DepositWorkflow workflow, CancellationToken ct = default)
        {
            await _context.DepositWorkflows.AddAsync(workflow, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(DepositWorkflow workflow, CancellationToken ct = default)
        {
            _context.DepositWorkflows.Update(workflow);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(DepositWorkflow workflow, CancellationToken ct = default)
        {
            _context.DepositWorkflows.Remove(workflow);
            await _context.SaveChangesAsync(ct);
        }
    }

    public class WorkflowStepRepository : IWorkflowStepRepository
    {
        private readonly ListingsDbContext _context;
        public WorkflowStepRepository(ListingsDbContext context) => _context = context;

        public Task<List<WorkflowStep>> GetByWorkflowIdAsync(int workflowId, CancellationToken ct = default) =>
            _context.WorkflowSteps
                .Where(s => s.WorkflowId == workflowId)
                .OrderBy(s => s.StepOrder)
                .ToListAsync(ct);

        public Task<WorkflowStep?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.WorkflowSteps.Include(s => s.Fields).FirstOrDefaultAsync(s => s.Id == id, ct);

        public async Task AddAsync(WorkflowStep step, CancellationToken ct = default)
        {
            await _context.WorkflowSteps.AddAsync(step, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(WorkflowStep step, CancellationToken ct = default)
        {
            _context.WorkflowSteps.Update(step);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(WorkflowStep step, CancellationToken ct = default)
        {
            _context.WorkflowSteps.Remove(step);
            await _context.SaveChangesAsync(ct);
        }
    }

    public class StepFieldRepository : IStepFieldRepository
    {
        private readonly ListingsDbContext _context;
        public StepFieldRepository(ListingsDbContext context) => _context = context;

        public Task<List<StepField>> GetByStepIdAsync(int stepId, CancellationToken ct = default) =>
            _context.StepFields
                .Where(f => f.StepId == stepId)
                .OrderBy(f => f.DisplayOrder)
                .ToListAsync(ct);

        public Task<StepField?> GetByIdAsync(int id, CancellationToken ct = default) =>
            _context.StepFields.FirstOrDefaultAsync(f => f.Id == id, ct);

        public async Task AddAsync(StepField field, CancellationToken ct = default)
        {
            await _context.StepFields.AddAsync(field, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(StepField field, CancellationToken ct = default)
        {
            _context.StepFields.Update(field);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(StepField field, CancellationToken ct = default)
        {
            _context.StepFields.Remove(field);
            await _context.SaveChangesAsync(ct);
        }
    }
}
