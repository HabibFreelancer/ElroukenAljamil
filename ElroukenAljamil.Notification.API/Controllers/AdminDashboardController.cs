using ElroukenAljamil.Notification.Application.DTOs;
using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Infrastructure.Hubs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Notification.API.Controllers
{
    /// <summary>
    /// Contrôleur d'administration pour le monitoring des notifications.
    /// Accès réservé aux utilisateurs avec le rôle Admin.
    /// Fournit les métriques de délivrance, les taux de succès/échec,
    /// et les statistiques en temps réel.
    /// </summary>
    [ApiController]
    [Route("api/notifications/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminDashboardController : ControllerBase
    {
        private readonly IDeliveryMetricsService _metricsService;
        private readonly IUserConnectionTracker _connectionTracker;

        public AdminDashboardController(
            IDeliveryMetricsService metricsService,
            IUserConnectionTracker connectionTracker)
        {
            _metricsService = metricsService;
            _connectionTracker = connectionTracker;
        }

        /// <summary>
        /// Récupère les métriques de délivrance pour une période donnée.
        /// Par défaut : dernières 24 heures.
        /// </summary>
        [HttpGet("dashboard")]
        [ProducesResponseType(typeof(DashboardDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetDashboard(
            [FromQuery] DateTime? from,
            [FromQuery] DateTime? to,
            CancellationToken ct)
        {
            var fromDate = from ?? DateTime.UtcNow.AddHours(-24);
            var toDate = to ?? DateTime.UtcNow;

            var metrics = await _metricsService.GetDashboardMetricsAsync(fromDate, toDate, ct);

            var dto = new DashboardDto
            {
                TotalSent = metrics.TotalSent,
                TotalFailed = metrics.TotalFailed,
                OverallSuccessRate = metrics.OverallSuccessRate,
                AverageDeliveryTimeMs = metrics.AverageDeliveryTimeMs,
                ByChannel = metrics.ByChannel.Select(c => new ChannelMetricsDto
                {
                    Channel = c.Channel,
                    Sent = c.Sent,
                    Failed = c.Failed,
                    SuccessRate = c.SuccessRate,
                    AvgDeliveryMs = c.AvgDeliveryMs
                }).ToList(),
                Hourly = metrics.Hourly.Select(h => new HourlyMetricsDto
                {
                    Hour = h.Hour,
                    Sent = h.Sent,
                    Failed = h.Failed
                }).ToList()
            };

            return Ok(dto);
        }

        /// <summary>
        /// Récupère le nombre d'utilisateurs actuellement connectés en temps réel.
        /// </summary>
        [HttpGet("online-count")]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetOnlineCount()
        {
            var count = await _connectionTracker.GetOnlineCountAsync();
            return Ok(new { onlineUsers = count, timestamp = DateTime.UtcNow });
        }
    }
}
