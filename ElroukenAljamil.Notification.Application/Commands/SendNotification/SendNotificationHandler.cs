using ElroukenAljamil.Notification.Application.Services;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.SendNotification
{
    public class SendNotificationHandler : IRequestHandler<SendNotificationCommand, bool>
    {
        private readonly INotificationOrchestrator _orchestrator;

        public SendNotificationHandler(INotificationOrchestrator orchestrator)
            => _orchestrator = orchestrator;

        public async Task<bool> Handle(SendNotificationCommand request, CancellationToken ct)
        {
            await _orchestrator.OrchestrateAsync(
                request.RecipientId, request.Type, request.Language, request.TemplateData, ct);
            return true;
        }
    }
}
