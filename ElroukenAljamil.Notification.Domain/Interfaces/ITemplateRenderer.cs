namespace ElroukenAljamil.Notification.Domain.Interfaces
{
    public interface ITemplateRenderer
    {
        Task<string> RenderAsync(string template, object model, CancellationToken ct = default);
    }
}
