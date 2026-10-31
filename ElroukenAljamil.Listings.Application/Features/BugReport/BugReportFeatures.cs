using ElroukenAljamil.BuildingBlocks.EventBus.Events.BugReport;
using ElroukenAljamil.Listings.Domain.Enums;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MassTransit;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Features.BugReport;

// ── DTOs ──────────────────────────────────────────────────────────────────────

public class CreateBugReportRequest
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BugSeverity Severity { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string? Module { get; set; }
    public string? Environment { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }
    public string? ScreenshotUrl { get; set; }
    public string? BrowserInfo { get; set; }
}

public class UpdateBugStatusRequest
{
    public BugStatus Status { get; set; }
    public string? AssignedToUserId { get; set; }
}

public class SetTestResultRequest
{
    public bool Passed { get; set; }
}

public class AddBugCommentRequest
{
    public string Content { get; set; } = string.Empty;
}

public class BugCommentDto
{
    public int Id { get; set; }
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class BugAttachmentDto
{
    public int Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StorageUrl { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSize { get; set; }
    public string UploadedByUserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class BugReportDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public BugSeverity Severity { get; set; }
    public string SeverityLabel { get; set; } = string.Empty;
    public BugStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? SubCategory { get; set; }
    public string? Module { get; set; }
    public string? Environment { get; set; }
    public string? StepsToReproduce { get; set; }
    public string? ExpectedBehavior { get; set; }
    public string? ActualBehavior { get; set; }
    public string? ScreenshotUrl { get; set; }
    public string? BrowserInfo { get; set; }
    public string ReportedByUserId { get; set; } = string.Empty;
    public string ReportedByUserName { get; set; } = string.Empty;
    public string? AssignedToUserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public IEnumerable<BugCommentDto> Comments { get; set; } = [];
    public IEnumerable<BugAttachmentDto> Attachments { get; set; } = [];
}

public class BugReportSummaryDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public BugSeverity Severity { get; set; }
    public string SeverityLabel { get; set; } = string.Empty;
    public BugStatus Status { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? Module { get; set; }
    public string ReportedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class PagedResult<T>
{
    public IEnumerable<T> Items { get; set; } = [];
    public int Total { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)Total / PageSize);
}

public class BugReportStatsDto
{
    public int Total { get; set; }
    public int New { get; set; }
    public int InProgress { get; set; }
    public int FixedAndToTest { get; set; }
    public int Resolved { get; set; }
    public int KO { get; set; }
    public int Closed { get; set; }
    public int Blocking { get; set; }
    public int Major { get; set; }
    public int Minor { get; set; }
}

// ── Helpers ───────────────────────────────────────────────────────────────────

internal static class BugReportMapper
{
    internal static string SeverityLabel(BugSeverity s) => s switch
    {
        BugSeverity.Blocking => "Bloquant",
        BugSeverity.Major    => "Majeur",
        BugSeverity.Minor    => "Mineur",
        _                    => s.ToString()
    };

    internal static string StatusLabel(BugStatus s) => s switch
    {
        BugStatus.New              => "Nouveau",
        BugStatus.InProgress       => "En cours",
        BugStatus.FixedAndToTest   => "Corrigé et à tester",
        BugStatus.Resolved         => "Résolu",
        BugStatus.KO               => "KO",
        BugStatus.Closed           => "Fermé",
        _                          => s.ToString()
    };

    internal static BugReportDto ToDto(Domain.Entities.BugReport b) => new()
    {
        Id                  = b.Id,
        Title               = b.Title,
        Description         = b.Description,
        Severity            = b.Severity,
        SeverityLabel       = SeverityLabel(b.Severity),
        Status              = b.Status,
        StatusLabel         = StatusLabel(b.Status),
        Category            = b.Category,
        SubCategory         = b.SubCategory,
        Module              = b.Module,
        Environment         = b.Environment,
        StepsToReproduce    = b.StepsToReproduce,
        ExpectedBehavior    = b.ExpectedBehavior,
        ActualBehavior      = b.ActualBehavior,
        ScreenshotUrl       = b.ScreenshotUrl,
        BrowserInfo         = b.BrowserInfo,
        ReportedByUserId    = b.ReportedByUserId,
        ReportedByUserName  = b.ReportedByUserName,
        AssignedToUserId    = b.AssignedToUserId,
        CreatedAt           = b.CreatedAt,
        UpdatedAt           = b.UpdatedAt,
        ClosedAt            = b.ClosedAt,
        Comments            = b.Comments.Select(c => new BugCommentDto
        {
            Id          = c.Id,
            AuthorId    = c.AuthorId,
            AuthorName  = c.AuthorName,
            Content     = c.Content,
            CreatedAt   = c.CreatedAt
        }),
        Attachments         = b.Attachments.Select(a => new BugAttachmentDto
        {
            Id               = a.Id,
            FileName         = a.FileName,
            StorageUrl       = a.StorageUrl,
            ContentType      = a.ContentType,
            FileSize         = a.FileSize,
            UploadedByUserId = a.UploadedByUserId,
            CreatedAt        = a.CreatedAt
        })
    };

    internal static BugReportSummaryDto ToSummary(Domain.Entities.BugReport b) => new()
    {
        Id                 = b.Id,
        Title              = b.Title,
        Severity           = b.Severity,
        SeverityLabel      = SeverityLabel(b.Severity),
        Status             = b.Status,
        StatusLabel        = StatusLabel(b.Status),
        Category           = b.Category,
        Module             = b.Module,
        ReportedByUserName = b.ReportedByUserName,
        CreatedAt          = b.CreatedAt
    };
}

// ── Queries ───────────────────────────────────────────────────────────────────

public record GetBugReportsQuery(
    int Page = 1,
    int PageSize = 20,
    BugStatus? Status = null,
    BugSeverity? Severity = null,
    string? Category = null,
    string? ReportedByUserId = null
) : IRequest<PagedResult<BugReportSummaryDto>>;

public class GetBugReportsQueryHandler : IRequestHandler<GetBugReportsQuery, PagedResult<BugReportSummaryDto>>
{
    private readonly IBugReportRepository _repo;
    public GetBugReportsQueryHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<PagedResult<BugReportSummaryDto>> Handle(GetBugReportsQuery q, CancellationToken ct)
    {
        var (items, total) = await _repo.GetPagedAsync(
            q.Page, q.PageSize, q.Status, q.Severity, q.Category, q.ReportedByUserId, ct);

        return new PagedResult<BugReportSummaryDto>
        {
            Items    = items.Select(BugReportMapper.ToSummary),
            Total    = total,
            Page     = q.Page,
            PageSize = q.PageSize
        };
    }
}

public record GetBugReportByIdQuery(int Id) : IRequest<BugReportDto?>;

public class GetBugReportByIdQueryHandler : IRequestHandler<GetBugReportByIdQuery, BugReportDto?>
{
    private readonly IBugReportRepository _repo;
    public GetBugReportByIdQueryHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<BugReportDto?> Handle(GetBugReportByIdQuery q, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(q.Id, ct);
        return bug is null ? null : BugReportMapper.ToDto(bug);
    }
}

public record GetBugReportStatsQuery : IRequest<BugReportStatsDto>;

public class GetBugReportStatsQueryHandler : IRequestHandler<GetBugReportStatsQuery, BugReportStatsDto>
{
    private readonly IBugReportRepository _repo;
    public GetBugReportStatsQueryHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<BugReportStatsDto> Handle(GetBugReportStatsQuery q, CancellationToken ct)
    {
        var (all, _) = await _repo.GetPagedAsync(1, int.MaxValue, ct: ct);
        var list = all.ToList();
        return new BugReportStatsDto
        {
            Total          = list.Count,
            New            = list.Count(b => b.Status == BugStatus.New),
            InProgress     = list.Count(b => b.Status == BugStatus.InProgress),
            FixedAndToTest = list.Count(b => b.Status == BugStatus.FixedAndToTest),
            Resolved       = list.Count(b => b.Status == BugStatus.Resolved),
            KO             = list.Count(b => b.Status == BugStatus.KO),
            Closed         = list.Count(b => b.Status == BugStatus.Closed),
            Blocking       = list.Count(b => b.Severity == BugSeverity.Blocking),
            Major          = list.Count(b => b.Severity == BugSeverity.Major),
            Minor          = list.Count(b => b.Severity == BugSeverity.Minor)
        };
    }
}

// ── Commands ──────────────────────────────────────────────────────────────────

public record CreateBugReportCommand(
    CreateBugReportRequest Request,
    string UserId,
    string UserName
) : IRequest<int>;

public class CreateBugReportCommandHandler : IRequestHandler<CreateBugReportCommand, int>
{
    private readonly IBugReportRepository _repo;
    private readonly IPublishEndpoint _publisher;

    public CreateBugReportCommandHandler(IBugReportRepository repo, IPublishEndpoint publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<int> Handle(CreateBugReportCommand cmd, CancellationToken ct)
    {
        var r = cmd.Request;
        var bug = Domain.Entities.BugReport.Create(
            r.Title, r.Description, r.Severity, r.Category,
            cmd.UserId, cmd.UserName,
            r.SubCategory, r.Module, r.Environment,
            r.StepsToReproduce, r.ExpectedBehavior, r.ActualBehavior,
            r.ScreenshotUrl, r.BrowserInfo);

        await _repo.AddAsync(bug, ct);
        await _repo.SaveChangesAsync(ct);

        try
        {
            await _publisher.Publish(new BugReportCreatedEvent
            {
                BugReportId       = bug.Id,
                Title             = bug.Title,
                SeverityLabel     = BugReportMapper.SeverityLabel(bug.Severity),
                Category          = bug.Category,
                Module            = bug.Module,
                ReportedByUserId  = bug.ReportedByUserId,
                ReportedByUserName = bug.ReportedByUserName
            }, ct);
        }
        catch { /* RabbitMQ optionnel */ }

        return bug.Id;
    }
}

public record UpdateBugStatusCommand(
    int Id,
    UpdateBugStatusRequest Request
) : IRequest<bool>;

public class UpdateBugStatusCommandHandler : IRequestHandler<UpdateBugStatusCommand, bool>
{
    private readonly IBugReportRepository _repo;
    private readonly IPublishEndpoint _publisher;

    public UpdateBugStatusCommandHandler(IBugReportRepository repo, IPublishEndpoint publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<bool> Handle(UpdateBugStatusCommand cmd, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(cmd.Id, ct);
        if (bug is null) return false;

        var oldStatusLabel = BugReportMapper.StatusLabel(bug.Status);
        bug.SetStatus(cmd.Request.Status, cmd.Request.AssignedToUserId);
        await _repo.SaveChangesAsync(ct);

        try
        {
            await _publisher.Publish(new BugStatusChangedEvent
            {
                BugReportId      = bug.Id,
                Title            = bug.Title,
                OldStatusLabel   = oldStatusLabel,
                NewStatusLabel   = BugReportMapper.StatusLabel(bug.Status),
                ReportedByUserId = bug.ReportedByUserId,
                ChangedByUserId  = cmd.Request.AssignedToUserId ?? string.Empty
            }, ct);
        }
        catch { /* RabbitMQ optionnel */ }

        return true;
    }
}

public record SetTestResultCommand(int Id, bool Passed) : IRequest<bool>;

public class SetTestResultCommandHandler : IRequestHandler<SetTestResultCommand, bool>
{
    private readonly IBugReportRepository _repo;
    private readonly IPublishEndpoint _publisher;

    public SetTestResultCommandHandler(IBugReportRepository repo, IPublishEndpoint publisher)
    {
        _repo = repo;
        _publisher = publisher;
    }

    public async Task<bool> Handle(SetTestResultCommand cmd, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(cmd.Id, ct);
        if (bug is null) return false;

        var oldStatusLabel = BugReportMapper.StatusLabel(bug.Status);
        bug.SetTestResult(cmd.Passed);
        await _repo.SaveChangesAsync(ct);

        try
        {
            await _publisher.Publish(new BugStatusChangedEvent
            {
                BugReportId      = bug.Id,
                Title            = bug.Title,
                OldStatusLabel   = oldStatusLabel,
                NewStatusLabel   = BugReportMapper.StatusLabel(bug.Status),
                ReportedByUserId = bug.ReportedByUserId,
                ChangedByUserId  = string.Empty
            }, ct);
        }
        catch { /* RabbitMQ optionnel */ }

        return true;
    }
}

public record AddBugCommentCommand(
    int BugReportId,
    string AuthorId,
    string AuthorName,
    string Content
) : IRequest<bool>;

public class AddBugCommentCommandHandler : IRequestHandler<AddBugCommentCommand, bool>
{
    private readonly IBugReportRepository _repo;
    public AddBugCommentCommandHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<bool> Handle(AddBugCommentCommand cmd, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(cmd.BugReportId, ct);
        if (bug is null) return false;

        bug.AddComment(cmd.AuthorId, cmd.AuthorName, cmd.Content);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}

// ── Attachments ───────────────────────────────────────────────────────────────

public record AddBugAttachmentCommand(
    int BugReportId,
    string FileName,
    string StorageUrl,
    string ContentType,
    long FileSize,
    string UploadedByUserId
) : IRequest<bool>;

public class AddBugAttachmentCommandHandler : IRequestHandler<AddBugAttachmentCommand, bool>
{
    private readonly IBugReportRepository _repo;
    public AddBugAttachmentCommandHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<bool> Handle(AddBugAttachmentCommand cmd, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(cmd.BugReportId, ct);
        if (bug is null) return false;

        bug.AddAttachment(cmd.FileName, cmd.StorageUrl, cmd.ContentType, cmd.FileSize, cmd.UploadedByUserId);
        await _repo.SaveChangesAsync(ct);
        return true;
    }
}

public record DeleteBugAttachmentCommand(int BugReportId, int AttachmentId) : IRequest<bool>;

public class DeleteBugAttachmentCommandHandler : IRequestHandler<DeleteBugAttachmentCommand, bool>
{
    private readonly IBugReportRepository _repo;
    public DeleteBugAttachmentCommandHandler(IBugReportRepository repo) => _repo = repo;

    public async Task<bool> Handle(DeleteBugAttachmentCommand cmd, CancellationToken ct)
    {
        var bug = await _repo.GetByIdAsync(cmd.BugReportId, ct);
        if (bug is null) return false;

        var removed = bug.RemoveAttachment(cmd.AttachmentId);
        if (!removed) return false;

        await _repo.SaveChangesAsync(ct);
        return true;
    }
}
