using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Notification.Application.Commands.UpdatePreferences;
using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Application.Queries.GetUserPreferences;
using ElroukenAljamil.Notification.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Notification.API.Controllers
{
    [ApiController]
    [Route("api/notifications/preferences")]
    [Authorize]
    public class PreferencesController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUser;

        public PreferencesController(IMediator mediator, ICurrentUserService currentUser)
        {
            _mediator = mediator;
            _currentUser = currentUser;
        }

        [HttpGet]
        public async Task<IActionResult> GetPreferences(CancellationToken ct = default)
        {
            var result = await _mediator.Send(new GetUserPreferencesQuery(_currentUser.UserId), ct);
            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePreferences([FromBody] UpdatePreferenceRequest request, CancellationToken ct = default)
        {
            if (!Enum.TryParse<NotificationType>(request.NotificationType, out var type))
                return BadRequest("Type de notification invalide.");

            await _mediator.Send(new UpdatePreferencesCommand(
                _currentUser.UserId, type,
                request.EmailEnabled, request.SmsEnabled,
                request.PushEnabled, request.InAppEnabled), ct);

            return NoContent();
        }
    }
}
