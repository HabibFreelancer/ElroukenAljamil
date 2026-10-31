using ElroukenAljamil.Media.Application.Commands.AssignMedia;
using ElroukenAljamil.Media.Application.Commands.DeleteMedia;
using ElroukenAljamil.Media.Application.Commands.UploadBugAttachment;
using ElroukenAljamil.Media.Application.Commands.UploadMedia;
using ElroukenAljamil.Media.Application.DTOs;
using ElroukenAljamil.Media.Application.Queries.GetMediaByListing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ElroukenAljamil.Media.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<MediaController> _logger;

        public MediaController(IMediator mediator, ILogger<MediaController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Upload d'une image.
        /// </summary>
        [HttpPost("upload")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)]
        [ProducesResponseType(typeof(UploadResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            var command = new UploadMediaCommand { File = file };
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Upload image échoué — {FileName} : {Error}", file?.FileName, result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Image uploadée avec succès — {MediaId} ({FileName})",
                result.Value!.MediaId, file?.FileName);
            return Created(string.Empty, result.Value);
        }

        /// <summary>
        /// Upload de plusieurs images en batch.
        /// </summary>
        [HttpPost("upload/batch")]
        [Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)]
        [ProducesResponseType(typeof(BatchUploadResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadBatch(List<IFormFile> files, CancellationToken ct)
        {
            if (files.Count == 0)
            {
                _logger.LogWarning("Upload batch — aucun fichier fourni");
                return BadRequest(new { error = "Aucun fichier fourni." });
            }

            if (files.Count > 15)
            {
                _logger.LogWarning("Upload batch — {Count} fichiers dépasse la limite de 15", files.Count);
                return BadRequest(new { error = "Maximum 15 fichiers par batch." });
            }

            var batchResult = new BatchUploadResultDto();

            foreach (var file in files)
            {
                var command = new UploadMediaCommand { File = file };
                var result = await _mediator.Send(command, ct);

                if (result.IsSuccess)
                {
                    batchResult.Uploaded.Add(result.Value!);
                }
                else
                {
                    _logger.LogWarning("Upload batch — échec fichier {FileName} : {Error}", file.FileName, result.Error);
                    batchResult.Errors.Add(new UploadErrorDto
                    {
                        FileName = file.FileName,
                        Error = result.Error!
                    });
                }
            }

            _logger.LogInformation("Upload batch terminé — {Uploaded} succès, {Errors} erreur(s)",
                batchResult.Uploaded.Count, batchResult.Errors.Count);
            return Ok(batchResult);
        }

        /// <summary>
        /// Assigne des médias à une annonce.
        /// </summary>
        [HttpPost("assign")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Assign([FromBody] AssignMediaRequest request, CancellationToken ct)
        {
            var command = new AssignMediaCommand
            {
                ListingId = request.ListingId,
                MediaIds = request.MediaIds
            };

            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Assignation médias échouée pour annonce {ListingId} — {Error}",
                    request.ListingId, result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("{Count} média(s) assigné(s) à l'annonce {ListingId}",
                request.MediaIds?.Count ?? 0, request.ListingId);
            return NoContent();
        }

        /// <summary>
        /// Récupère les images d'une annonce.
        /// </summary>
        [HttpGet("listing/{listingId:int}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<MediaFileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByListing(int listingId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaByListingQuery(listingId), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
        }

        /// <summary>
        /// Upload d'une pièce jointe pour un ticket bug (images, PDF, Word, Excel, PowerPoint, txt).
        /// </summary>
        [HttpPost("bug-attachment")]
        [Authorize(Roles = "Admin,Tester")]
        [RequestSizeLimit(20 * 1024 * 1024)]
        [ProducesResponseType(typeof(UploadResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UploadBugAttachment(IFormFile file, CancellationToken ct)
        {
            var result = await _mediator.Send(new UploadBugAttachmentCommand { File = file }, ct);

            if (!result.IsSuccess)
            {
                _logger.LogWarning("Upload bug attachment échoué — {FileName} : {Error}", file?.FileName, result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Bug attachment uploadé — {MediaId} ({FileName})",
                result.Value!.MediaId, file?.FileName);
            return Created(string.Empty, result.Value);
        }

        /// <summary>
        /// Supprime un média.
        /// </summary>
        [HttpDelete("{mediaId:guid}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid mediaId, CancellationToken ct)
        {
            var result = await _mediator.Send(new DeleteMediaCommand(mediaId), ct);

            if (!result.IsSuccess)
            {
                if (result.Error!.Contains("introuvable"))
                {
                    _logger.LogWarning("Suppression média {MediaId} — introuvable", mediaId);
                    return NotFound(new { error = result.Error });
                }
                _logger.LogWarning("Suppression média {MediaId} échouée — {Error}", mediaId, result.Error);
                return BadRequest(new { error = result.Error });
            }

            _logger.LogInformation("Média {MediaId} supprimé", mediaId);
            return NoContent();
        }
    }

}
