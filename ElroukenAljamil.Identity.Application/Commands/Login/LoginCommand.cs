using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using MediatR;


namespace ElroukenAljamil.Identity.Application.Commands.Login
{
    public record LoginCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
    }
}
