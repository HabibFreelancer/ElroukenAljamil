using ElroukenAljamil.Identity.Domain.Enums;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Identity.API.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IdentityDbContext _db;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IdentityDbContext db, ILogger<UsersController> logger)
    {
        _db = db;
        _logger = logger;
    }

    /// <summary>Retourne les IDs de tous les admins — utilisé par Notification API.</summary>
    [HttpGet("admins")]
    [AllowAnonymous] // appelé service-to-service depuis Notification API
    public async Task<IActionResult> GetAdminIds(CancellationToken ct)
    {
        var ids = await _db.Users
            .Where(u => u.Role == UserRole.Admin && u.Status == Domain.Enums.UserStatus.Active)
            .Select(u => u.Id)
            .ToListAsync(ct);

        _logger.LogDebug("GetAdminIds — {Count} admin(s) retourné(s)", ids.Count);
        return Ok(ids);
    }

    /// <summary>Retourne les infos d'un utilisateur par ID — utilisé par Notification API.</summary>
    [HttpGet("{id:guid}/info")]
    [AllowAnonymous] // appelé service-to-service
    public async Task<IActionResult> GetUserInfo(Guid id, CancellationToken ct)
    {
        var user = await _db.Users
            .Where(u => u.Id == id)
            .Select(u => new
            {
                userId      = u.Id,
                fullName    = u.FirstName + " " + u.LastName,
                email       = u.Email,
                phoneNumber = u.PhoneNumber
            })
            .FirstOrDefaultAsync(ct);

        if (user is null)
        {
            _logger.LogWarning("GetUserInfo — utilisateur {UserId} introuvable", id);
            return NotFound();
        }
        return Ok(user);
    }
}
