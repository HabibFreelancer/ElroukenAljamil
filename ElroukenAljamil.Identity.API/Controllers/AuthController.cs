using ElroukenAljamil.Identity.Application.Commands.Login;
using ElroukenAljamil.Identity.Application.Commands.RefreshToken;
using ElroukenAljamil.Identity.Application.Commands.Register;
using ElroukenAljamil.Identity.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Identity.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Inscription d'un nouvel utilisateur.
        /// </summary>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Created(string.Empty, result.Value);
        }

        /// <summary>
        /// Connexion d'un utilisateur existant.
        /// </summary>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return Unauthorized(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Renouvellement du token d'accès via le refresh token.
        /// </summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(AuthResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return Unauthorized(new { error = result.Error });

            return Ok(result.Value);
        }
    }
}
