using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Media.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ElroukenAljamil.Media.Application.Commands.UploadMedia
{
    public record UploadMediaCommand : IRequest<Result<UploadResultDto>>
    {
        public IFormFile File { get; init; } = null!;
    }
}
