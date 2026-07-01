using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.RefreshToken
{
    public record RefreshTokenCommand : IRequest<Result<AuthResponseDto>>
    {
        public string AccessToken { get; init; } = string.Empty;
        public string RefreshToken { get; init; } = string.Empty;
    }
}
