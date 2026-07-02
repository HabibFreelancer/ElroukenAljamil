using ElroukenAljamil.Media.Application.Commands.AssignMedia;
using ElroukenAljamil.Media.Application.Commands.DeleteMedia;
using ElroukenAljamil.Media.Application.Commands.UploadMedia;
using ElroukenAljamil.Media.Application.DTOs;
using ElroukenAljamil.Media.Application.Queries.GetMediaByListing;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ElroukenAljamil.Media.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MediaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MediaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Upload d'une image.
        /// </summary>
        [HttpPost("upload")]
        [Authorize]
        [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
        [ProducesResponseType(typeof(UploadResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Upload(IFormFile file, CancellationToken ct)
        {
            var command = new UploadMediaCommand { File = file };
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Created(string.Empty, result.Value);
        }

        /// <summary>
        /// Upload de plusieurs images en batch.
        /// </summary>
        [HttpPost("upload/batch")]
        [Authorize]
        [RequestSizeLimit(50 * 1024 * 1024)] // 50 MB total
        [ProducesResponseType(typeof(BatchUploadResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> UploadBatch(List<IFormFile> files, CancellationToken ct)
        {
            if (files.Count == 0)
                return BadRequest(new { error = "Aucun fichier fourni." });

            if (files.Count > 15)
                return BadRequest(new { error = "Maximum 15 fichiers par batch." });

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
                    batchResult.Errors.Add(new UploadErrorDto
                    {
                        FileName = file.FileName,
                        Error = result.Error!
                    });
                }
            }

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
                return BadRequest(new { error = result.Error });

            return NoContent();
        }

        /// <summary>
        /// Récupère les images d'une annonce.
        /// </summary>
        [HttpGet("listing/{listingId:guid}")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(List<MediaFileDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByListing(Guid listingId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetMediaByListingQuery(listingId), ct);

            if (!result.IsSuccess)
                return BadRequest(new { error = result.Error });

            return Ok(result.Value);
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
                    return NotFound(new { error = result.Error });
                return BadRequest(new { error = result.Error });
            }

            return NoContent();
        }
    }

}
