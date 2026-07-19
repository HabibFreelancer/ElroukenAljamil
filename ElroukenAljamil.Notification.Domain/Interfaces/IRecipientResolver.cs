namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface IRecipientResolver
    {
        Task<RecipientInfo?> ResolveAsync(Guid userId, CancellationToken ct = default);
    }

    public record RecipientInfo(Guid UserId, string FullName, string Email, string? PhoneNumber);
}
