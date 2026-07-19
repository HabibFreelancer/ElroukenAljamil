using ElroukenAljamil.Notification.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Commands.MarkAsRead
{
    public class MarkAsReadHandler : IRequestHandler<MarkAsReadCommand, bool>
    {
        private readonly INotificationRepository _repository;

        public MarkAsReadHandler(INotificationRepository repository)
            => _repository = repository;

        public async Task<bool> Handle(MarkAsReadCommand request, CancellationToken ct)
        {
            var notification = await _repository.GetByIdAsync(request.NotificationId, ct);
            if (notification is null || notification.RecipientId != request.UserId)
                return false;

            notification.MarkAsRead();
            await _repository.UpdateAsync(notification, ct);
            return true;
        }
    }
}
