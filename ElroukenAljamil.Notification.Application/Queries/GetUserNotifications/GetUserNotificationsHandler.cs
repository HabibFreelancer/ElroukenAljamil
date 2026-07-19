using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Notification.Application.Queries.GetUserNotifications
{
    public class GetUserNotificationsHandler : IRequestHandler<GetUserNotificationsQuery, IReadOnlyList<NotificationDto>>
    {
        private readonly INotificationRepository _repository;

        public GetUserNotificationsHandler(INotificationRepository repository)
            => _repository = repository;

        public async Task<IReadOnlyList<NotificationDto>> Handle(GetUserNotificationsQuery request, CancellationToken ct)
        {
            var records = await _repository.GetByRecipientAsync(request.UserId, request.Page, request.PageSize, ct);
            return records.Select(n => new NotificationDto
            {
                Id = n.Id,
                Type = n.Type.ToString(),
                Channel = n.Channel.ToString(),
                Title = n.Title,
                Body = n.Body,
                Status = n.Status.ToString(),
                Metadata = n.Metadata,
                CreatedAt = n.CreatedAt,
                ReadAt = n.ReadAt
            }).ToList();
        }
    }
}
