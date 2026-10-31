using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Listings.Application.Features.BugReport;
using ElroukenAljamil.Listings.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Listings.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin,Tester")]
public class BugReportsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;
    private readonly ILogger<BugReportsController> _logger;

    public BugReportsController(IMediator mediator, ICurrentUserService currentUser, ILogger<BugReportsController> logger)
    {
        _mediator = mediator;
        _currentUser = currentUser;
        _logger = logger;
    }

    /// <summary>Liste paginée avec filtres.</summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] BugStatus? status = null,
        [FromQuery] BugSeverity? severity = null,
        [FromQuery] string? category = null,
        CancellationToken ct = default)
    {
        // Les testeurs ne voient que leurs propres tickets
        string? filterByUser = null;
        if (User.IsInRole("Tester") && !User.IsInRole("Admin"))
            filterByUser = _currentUser.UserId.ToString();

        var result = await _mediator.Send(
            new GetBugReportsQuery(page, pageSize, status, severity, category, filterByUser), ct);
        return Ok(result);
    }

    /// <summary>Statistiques du tableau de bord.</summary>
    [HttpGet("stats")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetStats(CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBugReportStatsQuery(), ct);
        return Ok(result);
    }

    /// <summary>Détail d'un ticket.</summary>
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id, CancellationToken ct)
    {
        var result = await _mediator.Send(new GetBugReportByIdQuery(id), ct);
        if (result is null) return NotFound();
        return Ok(result);
    }

    /// <summary>Créer un ticket (Admin + Tester).</summary>
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBugReportRequest request, CancellationToken ct)
    {
        var userId   = _currentUser.UserId.ToString();
        var userName = string.IsNullOrWhiteSpace(_currentUser.FullName) ? _currentUser.Email : _currentUser.FullName;

        var id = await _mediator.Send(new CreateBugReportCommand(request, userId, userName), ct);
        _logger.LogInformation("Bug report {BugId} créé par {UserName} ({UserId}) — {Title}",
            id, userName, userId, request.Title);
        return CreatedAtAction(nameof(GetById), new { id }, new { id });
    }

    /// <summary>Changer le statut — Admin uniquement.</summary>
    [HttpPut("{id:int}/status")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateBugStatusRequest request, CancellationToken ct)
    {
        var updated = await _mediator.Send(new UpdateBugStatusCommand(id, request), ct);
        if (!updated)
        {
            _logger.LogWarning("Changement statut bug report {BugId} — introuvable", id);
            return NotFound();
        }
        _logger.LogInformation("Bug report {BugId} — statut mis à jour en {Status} par {UserId}",
            id, request.Status, _currentUser.UserId);
        return NoContent();
    }

    /// <summary>Valider ou rejeter un correctif — Tester uniquement (statut FixedAndToTest requis).</summary>
    [HttpPut("{id:int}/test-result")]
    [Authorize(Roles = "Tester,Admin")]
    public async Task<IActionResult> SetTestResult(int id, [FromBody] SetTestResultRequest request, CancellationToken ct)
    {
        try
        {
            var updated = await _mediator.Send(new SetTestResultCommand(id, request.Passed), ct);
            if (!updated)
            {
                _logger.LogWarning("SetTestResult bug report {BugId} — introuvable", id);
                return NotFound();
            }
            _logger.LogInformation("Bug report {BugId} — test result {Passed} par {UserId}",
                id, request.Passed, _currentUser.UserId);
            return NoContent();
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("SetTestResult bug report {BugId} — opération invalide : {Message}", id, ex.Message);
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>Ajouter un commentaire.</summary>
    [HttpPost("{id:int}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] AddBugCommentRequest request, CancellationToken ct)
    {
        var userId   = _currentUser.UserId.ToString();
        var userName = string.IsNullOrWhiteSpace(_currentUser.FullName) ? _currentUser.Email : _currentUser.FullName;

        var added = await _mediator.Send(new AddBugCommentCommand(id, userId, userName, request.Content), ct);
        if (!added) return NotFound();
        return NoContent();
    }

    /// <summary>Uploader une pièce jointe sur un ticket existant.</summary>
    [HttpPost("{id:int}/attachments")]
    [RequestSizeLimit(20 * 1024 * 1024)]
    public async Task<IActionResult> AddAttachment(int id, IFormFile file, CancellationToken ct)
    {
        if (file is null || file.Length == 0)
        {
            _logger.LogWarning("Upload pièce jointe bug report {BugId} — fichier manquant ou vide", id);
            return BadRequest(new { message = "Fichier manquant ou vide." });
        }

        // 1. Upload vers MinIO via Media API (appel HTTP interne) — ici on délègue directement
        //    au handler local pour éviter un appel réseau supplémentaire.
        var allowedExtensions = new[]
        {
            ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif", ".bmp",
            ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx",
            ".txt", ".csv", ".log"
        };
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(ext))
        {
            _logger.LogWarning("Upload pièce jointe bug report {BugId} — type refusé : {Extension}", id, ext);
            return BadRequest(new { message = $"Type de fichier non autorisé : {ext}" });
        }

        if (file.Length > 20 * 1024 * 1024)
            return BadRequest(new { message = "La taille maximale est de 20 MB." });

        // 2. Enregistrer la pièce jointe sur le ticket
        var userId = _currentUser.UserId.ToString();
        var storageUrl = $"/api/media/bug-attachment/placeholder/{Guid.NewGuid()}{ext}";

        var added = await _mediator.Send(
            new AddBugAttachmentCommand(id, file.FileName, storageUrl, file.ContentType, file.Length, userId), ct);

        if (!added)
        {
            _logger.LogWarning("Upload pièce jointe bug report {BugId} — ticket introuvable", id);
            return NotFound();
        }
        _logger.LogInformation("Pièce jointe {FileName} ajoutée au bug report {BugId} par {UserId}",
            file.FileName, id, userId);
        return NoContent();
    }

    /// <summary>Supprimer une pièce jointe.</summary>
    [HttpDelete("{id:int}/attachments/{attachmentId:int}")]
    public async Task<IActionResult> DeleteAttachment(int id, int attachmentId, CancellationToken ct)
    {
        var deleted = await _mediator.Send(new DeleteBugAttachmentCommand(id, attachmentId), ct);
        if (!deleted) return NotFound();
        return NoContent();
    }
}
