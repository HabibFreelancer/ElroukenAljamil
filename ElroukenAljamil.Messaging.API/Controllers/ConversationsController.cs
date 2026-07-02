using ElroukenAljamil.Messaging.Application.Commands.MarkAsRead;
using ElroukenAljamil.Messaging.Application.Commands.SendMessage;
using ElroukenAljamil.Messaging.Application.Commands.StartConversation;
using ElroukenAljamil.Messaging.Application.Commands.EditMessage;
using ElroukenAljamil.Messaging.Application.Commands.DeleteMessage;
using ElroukenAljamil.Messaging.Application.DTOs;
using ElroukenAljamil.Messaging.Application.Queries.GetConversationDetail;
using ElroukenAljamil.Messaging.Application.Queries.GetConversations;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Messaging.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConversationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ConversationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Récupère la liste des conversations de l'utilisateur connecté.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ConversationListDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetConversations(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var query = new GetConversationsQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Récupère le détail d'une conversation avec tous ses messages.
        /// </summary>
        [HttpGet("{conversationId:guid}")]
        [ProducesResponseType(typeof(ConversationDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetConversation(Guid conversationId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetConversationDetailQuery(conversationId), ct);

            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("introuvable"))
                    return NotFound(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Démarre une nouvelle conversation (ou envoie un message dans une existante).
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> StartConversation(
            [FromBody] StartConversationCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return CreatedAtAction(
                nameof(GetConversation),
                new { conversationId = result.Value },
                result.Value);
        }

        /// <summary>
        /// Envoie un message dans une conversation existante.
        /// </summary>
        [HttpPost("{conversationId:guid}/messages")]
        [ProducesResponseType(typeof(MessageDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> SendMessage(
            Guid conversationId,
            [FromBody] SendMessageRequest request,
            CancellationToken ct)
        {
            var command = new SendMessageCommand
            {
                ConversationId = conversationId,
                Content = request.Content
            };

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Created(string.Empty, result.Value);
        }

        /// <summary>
        /// Marque tous les messages de la conversation comme lus.
        /// </summary>
        [HttpPost("{conversationId:guid}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MarkAsRead(Guid conversationId, CancellationToken ct)
        {
            var result = await _mediator.Send(new MarkAsReadCommand(conversationId), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Modifie un message (dans les 15 minutes après envoi).
        /// </summary>
        [HttpPut("{conversationId:guid}/messages/{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> EditMessage(
            Guid conversationId,
            Guid messageId,
            [FromBody] EditMessageRequest request,
            CancellationToken ct)
        {
            var command = new EditMessageCommand
            {
                ConversationId = conversationId,
                MessageId = messageId,
                NewContent = request.Content
            };

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Supprime un message (soft delete).
        /// </summary>
        [HttpDelete("{conversationId:guid}/messages/{messageId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteMessage(
            Guid conversationId,
            Guid messageId,
            CancellationToken ct)
        {
            var command = new DeleteMessageCommand
            {
                ConversationId = conversationId,
                MessageId = messageId
            };

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return NoContent();
        }
    }

    // --- Request DTOs (simples, sans logique) ---

    public record SendMessageRequest
    {
        public string Content { get; init; } = string.Empty;
    }

    public record EditMessageRequest
    {
        public string Content { get; init; } = string.Empty;
    }

}
