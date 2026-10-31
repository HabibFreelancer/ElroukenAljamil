using ElroukenAljamil.BuildingBlocks.Common.Domain;
using ElroukenAljamil.Listings.Domain.Enums;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class BugReport : AggregateRoot
{
    public new int Id { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public BugSeverity Severity { get; private set; }
    public BugStatus Status { get; private set; }
    public string Category { get; private set; } = string.Empty;
    public string? SubCategory { get; private set; }
    public string? Module { get; private set; }
    public string? Environment { get; private set; }
    public string? StepsToReproduce { get; private set; }
    public string? ExpectedBehavior { get; private set; }
    public string? ActualBehavior { get; private set; }
    public string? ScreenshotUrl { get; private set; }
    public string? BrowserInfo { get; private set; }
    public string ReportedByUserId { get; private set; } = string.Empty;
    public string ReportedByUserName { get; private set; } = string.Empty;
    public string? AssignedToUserId { get; private set; }
    public DateTime? ClosedAt { get; private set; }

    private readonly List<BugComment> _comments = new();
    public IReadOnlyCollection<BugComment> Comments => _comments.AsReadOnly();

    private readonly List<BugAttachment> _attachments = new();
    public IReadOnlyCollection<BugAttachment> Attachments => _attachments.AsReadOnly();

    private BugReport() { }

    public static BugReport Create(
        string title,
        string description,
        BugSeverity severity,
        string category,
        string reportedByUserId,
        string reportedByUserName,
        string? subCategory = null,
        string? module = null,
        string? environment = null,
        string? stepsToReproduce = null,
        string? expectedBehavior = null,
        string? actualBehavior = null,
        string? screenshotUrl = null,
        string? browserInfo = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(description);
        ArgumentException.ThrowIfNullOrWhiteSpace(category);

        return new BugReport
        {
            Title = title,
            Description = description,
            Severity = severity,
            Status = BugStatus.New,
            Category = category,
            SubCategory = subCategory,
            Module = module,
            Environment = environment,
            StepsToReproduce = stepsToReproduce,
            ExpectedBehavior = expectedBehavior,
            ActualBehavior = actualBehavior,
            ScreenshotUrl = screenshotUrl,
            BrowserInfo = browserInfo,
            ReportedByUserId = reportedByUserId,
            ReportedByUserName = reportedByUserName
        };
    }

    /// <summary>Admin only — transition libre entre statuts.</summary>
    public void SetStatus(BugStatus newStatus, string? assignedToUserId = null)
    {
        Status = newStatus;
        if (assignedToUserId != null)
            AssignedToUserId = assignedToUserId;
        if (newStatus == BugStatus.Closed)
            ClosedAt = DateTime.UtcNow;
    }

    /// <summary>Tester only — après FixedAndToTest, marque Resolved ou KO.</summary>
    public void SetTestResult(bool passed)
    {
        if (Status != BugStatus.FixedAndToTest)
            throw new InvalidOperationException("Le ticket doit être en statut 'Corrigé et à tester' pour valider.");

        Status = passed ? BugStatus.Resolved : BugStatus.KO;
    }

    public void AddComment(string authorId, string authorName, string content)
    {
        _comments.Add(BugComment.Create(Id, authorId, authorName, content));
    }

    public void UpdateScreenshot(string screenshotUrl)
    {
        ScreenshotUrl = screenshotUrl;
    }

    public void AddAttachment(string fileName, string storageUrl, string contentType, long fileSize, string uploadedByUserId)
    {
        _attachments.Add(BugAttachment.Create(Id, fileName, storageUrl, contentType, fileSize, uploadedByUserId));
    }

    public bool RemoveAttachment(int attachmentId)
    {
        var att = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        if (att is null) return false;
        _attachments.Remove(att);
        return true;
    }
}
