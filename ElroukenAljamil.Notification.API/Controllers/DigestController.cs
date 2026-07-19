using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Domain.Entities;
using ElroukenAljamil.Notification.Domain.Enums;
using ElroukenAljamil.Notification.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Notification.API.Controllers
{
    /// <summary>
    /// Contrôleur pour la gestion des digest emails de l'utilisateur.
    /// Permet de configurer la fréquence et les préférences de résumé.
    /// </summary>
    [ApiController]
    [Route("api/notifications/digest")]
    [Authorize]
    public class DigestController : ControllerBase
    {
        private readonly IDigestScheduleRepository _digestRepository;
        private readonly ICurrentUserService _currentUser;

        public DigestController(
            IDigestScheduleRepository digestRepository,
            ICurrentUserService currentUser)
        {
            _digestRepository = digestRepository;
            _currentUser = currentUser;
        }

        /// <summary>
        /// Récupère la configuration digest de l'utilisateur connecté.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(DigestConfigDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDigestConfig(CancellationToken ct)
        {
            var schedule = await _digestRepository.GetByUserIdAsync(_currentUser.UserId, ct);

            if (schedule is null)
            {
                // Retourner les valeurs par défaut
                return Ok(new DigestConfigDto
                {
                    Frequency = "Daily",
                    PreferredHour = 8,
                    PreferredDay = "Monday",
                    TimeZone = "Europe/Paris",
                    IsActive = false
                });
            }

            return Ok(new DigestConfigDto
            {
                Frequency = schedule.Frequency.ToString(),
                PreferredHour = schedule.PreferredHour,
                PreferredDay = schedule.PreferredDay.ToString(),
                TimeZone = schedule.TimeZone,
                IsActive = schedule.IsActive
            });
        }

        /// <summary>
        /// Met à jour ou crée la configuration digest.
        /// </summary>
        [HttpPut]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateDigestConfig(
            [FromBody] DigestConfigDto request, CancellationToken ct)
        {
            if (!Enum.TryParse<DigestFrequency>(request.Frequency, true, out var frequency))
                return BadRequest(new { error = "Fréquence invalide. Valeurs possibles : Daily, Weekly." });

            if (!Enum.TryParse<DayOfWeek>(request.PreferredDay, true, out var preferredDay))
                return BadRequest(new { error = "Jour invalide." });

            var existing = await _digestRepository.GetByUserIdAsync(_currentUser.UserId, ct);

            if (existing is not null)
            {
                existing.Update(frequency, request.PreferredHour, preferredDay, request.TimeZone);

                if (request.IsActive)
                    existing.Activate();
                else
                    existing.Deactivate();

                await _digestRepository.UpdateAsync(existing, ct);
            }
            else
            {
                var schedule = DigestSchedule.Create(
                    _currentUser.UserId, frequency,
                    request.PreferredHour, preferredDay, request.TimeZone);

                if (!request.IsActive)
                    schedule.Deactivate();

                await _digestRepository.AddAsync(schedule, ct);
            }

            return NoContent();
        }
    }

}
