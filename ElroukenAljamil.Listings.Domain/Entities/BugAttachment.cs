using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class BugAttachment : BaseEntity
{
    public new int Id { get; private set; }
    public int BugReportId { get; private set; }
    public string FileName { get; private set; } = string.Empty;
    public string StorageUrl { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long FileSize { get; private set; }
    public string UploadedByUserId { get; private set; } = string.Empty;

    public BugReport BugReport { get; private set; } = null!;

    private BugAttachment() { }

    public static BugAttachment Create(
        int bugReportId,
        string fileName,
        string storageUrl,
        string contentType,
        long fileSize,
        string uploadedByUserId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fileName);
        ArgumentException.ThrowIfNullOrWhiteSpace(storageUrl);

        return new BugAttachment
        {
            BugReportId       = bugReportId,
            FileName          = fileName,
            StorageUrl        = storageUrl,
            ContentType       = contentType,
            FileSize          = fileSize,
            UploadedByUserId  = uploadedByUserId,
            CreatedAt         = DateTime.UtcNow
        };
    }
}
