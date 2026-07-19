using ElroukenAljamil.Notification.Domain.Interfaces;
using Scriban;

namespace ElroukenAljamil.Notification.Infrastructure.Templates
{
    public class ScribanTemplateRenderer : ITemplateRenderer
    {
        public async Task<string> RenderAsync(string templateText, object model, CancellationToken ct = default)
        {
            var template = Template.Parse(templateText);
            return await template.RenderAsync(model);
        }
    }
}
