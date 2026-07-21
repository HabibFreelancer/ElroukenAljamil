using ElroukenAljamil.Listings.Domain.Entities;

namespace ElroukenAljamil.Listings.Domain.Interfaces
{
    public interface IDepositWorkflowRepository
    {
        Task<List<DepositWorkflow>> GetAllAsync(CancellationToken ct = default);
        Task<DepositWorkflow?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<DepositWorkflow?> GetByCategoryIdAsync(int categoryId, CancellationToken ct = default);
        Task AddAsync(DepositWorkflow workflow, CancellationToken ct = default);
        Task UpdateAsync(DepositWorkflow workflow, CancellationToken ct = default);
        Task DeleteAsync(DepositWorkflow workflow, CancellationToken ct = default);
    }

    public interface IWorkflowStepRepository
    {
        Task<List<WorkflowStep>> GetByWorkflowIdAsync(int workflowId, CancellationToken ct = default);
        Task<WorkflowStep?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(WorkflowStep step, CancellationToken ct = default);
        Task UpdateAsync(WorkflowStep step, CancellationToken ct = default);
        Task DeleteAsync(WorkflowStep step, CancellationToken ct = default);
    }

    public interface IStepFieldRepository
    {
        Task<List<StepField>> GetByStepIdAsync(int stepId, CancellationToken ct = default);
        Task<StepField?> GetByIdAsync(int id, CancellationToken ct = default);
        Task AddAsync(StepField field, CancellationToken ct = default);
        Task UpdateAsync(StepField field, CancellationToken ct = default);
        Task DeleteAsync(StepField field, CancellationToken ct = default);
    }
}
