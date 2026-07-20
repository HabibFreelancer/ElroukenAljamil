using System.Security.Claims;
using ElroukenAljamil.Identity.Application.Commands.CheckEmail;
using ElroukenAljamil.Identity.Application.Commands.Login;
using ElroukenAljamil.Identity.Application.Commands.RefreshToken;
using ElroukenAljamil.Identity.Application.Commands.Register;
using ElroukenAljamil.Identity.Application.Commands.SendEmailCode;
using ElroukenAljamil.Identity.Application.Commands.SendSmsCode;
using ElroukenAljamil.Identity.Application.Commands.VerifyEmailCode;
using ElroukenAljamil.Identity.Application.Commands.VerifyPhone;
using ElroukenAljamil.Identity.Application.DTOs;
using ElroukenAljamil.Identity.Application.Queries.GetCurrentUser;
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
        public AuthController(IMediator mediator) => _mediator = mediator;

        /// <summary>Vérifie si un email existe déjà.</summary>
        [HttpPost("check-email")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequest request, CancellationToken ct)
        {
            var exists = await _mediator.Send(new CheckEmailCommand(request.Email), ct);
            return Ok(new { exists });
        }

        /// <summary>Envoie un OTP 5 chiffres valable 10 min à l'email.</summary>
        [HttpPost("send-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request, CancellationToken ct)
        {
            var maskedEmail = await _mediator.Send(new SendEmailCodeCommand(request.Email), ct);
            return Ok(new { message = "Code envoyé.", maskedEmail });
        }

        /// <summary>Vérifie le code OTP email.</summary>
        [HttpPost("verify-code")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyCode([FromBody] VerifyCodeRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new VerifyEmailCodeCommand(request.Email, request.Code), ct);
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });
            return Ok(new { message = "Email vérifié." });
        }

        /// <summary>Inscription avec type de compte (personal / pro).</summary>
        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });
            return Created(string.Empty, result.Value);
        }

        /// <summary>Envoie un OTP 6 chiffres par SMS pour vérification téléphone.</summary>
        [HttpPost("send-sms-code")]
        [AllowAnonymous]
        public async Task<IActionResult> SendSmsCode([FromBody] SendSmsCodeRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new SendSmsCodeCommand(request.Email, request.Phone), ct);
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });
            return Ok(new { message = "SMS envoyé." });
        }

        /// <summary>Vérifie le code SMS → active le compte et retourne JWT.</summary>
        [HttpPost("verify-phone")]
        [AllowAnonymous]
        public async Task<IActionResult> VerifyPhone([FromBody] VerifyCodeRequest request, CancellationToken ct)
        {
            var result = await _mediator.Send(new VerifyPhoneCommand(request.Email, request.Code), ct);
            if (!result.IsSuccess) return BadRequest(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>Connexion email/mot de passe → JWT.</summary>
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess) return Unauthorized(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>Renouvellement du token via refresh token.</summary>
        [HttpPost("refresh")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            if (!result.IsSuccess) return Unauthorized(new { error = result.Error });
            return Ok(result.Value);
        }

        /// <summary>Profil de l'utilisateur connecté.</summary>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> Me(CancellationToken ct)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!Guid.TryParse(userIdStr, out var userId)) return Unauthorized();

            var profile = await _mediator.Send(new GetCurrentUserQuery(userId), ct);
            return profile is null ? Unauthorized() : Ok(profile);
        }
    }
}
