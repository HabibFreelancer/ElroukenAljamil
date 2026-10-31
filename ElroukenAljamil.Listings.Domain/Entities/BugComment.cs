using ElroukenAljamil.BuildingBlocks.Common.Domain;

namespace ElroukenAljamil.Listings.Domain.Entities;

public class BugComment : BaseEntity
{
    public new int Id { get; private set; }
    public int BugReportId { get; private set; }
    public string AuthorId { get; private set; } = string.Empty;
    public string AuthorName { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;

    public BugReport BugReport { get; private set; } = null!;

    private BugComment() { }

    public static BugComment Create(int bugReportId, string authorId, string authorName, string content)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(content);
        return new BugComment
        {
            BugReportId = bugReportId,
            AuthorId = authorId,
            AuthorName = authorName,
            Content = content
        };
    }
}
