using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Notification.Application.Commands.MarkAsRead;
using ElroukenAljamil.Notification.Application.Commands.SendNotification;
using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Application.Queries.GetUserNotifications;
using ElroukenAljamil.Notification.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Notification.API.Controllers
{
    [ApiController]
    [Route("api/notifications")]
    [Authorize]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;
        private readonly ILogger<NotificationsController> _logger;

        public NotificationsController(IMediator mediator, ICurrentUserService currentUser, ILogger<NotificationsController> logger)
        {
            _mediator = mediator;
            _currentUser = currentUser;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;
            var result = await _mediator.Send(new GetUserNotificationsQuery(userId, page, pageSize), ct);
            return Ok(result);
        }

        [HttpPost("{id:guid}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id, CancellationToken ct = default)
        {
            var userId = _currentUser.UserId;
            var success = await _mediator.Send(new MarkAsReadCommand(id, userId), ct);
            if (!success)
            {
                _logger.LogWarning("MarkAsRead notification {NotificationId} — introuvable pour {UserId}", id, userId);
                return NotFound();
            }
            return NoContent();
        }

        [HttpPost("send")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Send([FromBody] SendNotificationRequest request, CancellationToken ct = default)
        {
            _logger.LogInformation("Envoi manuel notification {Type} à {RecipientId} par admin",
                request.Type, request.RecipientId);
            await _mediator.Send(new SendNotificationCommand(
                request.RecipientId, request.Type, request.Language, request.TemplateData), ct);
            return Accepted();
        }
    }
}
