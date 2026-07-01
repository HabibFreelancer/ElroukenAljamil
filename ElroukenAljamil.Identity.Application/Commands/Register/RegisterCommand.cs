using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Identity.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Identity.Application.Commands.Register
{
    public record RegisterCommand : IRequest<Result<AuthResponseDto>>
    {
        public string Email { get; init; } = string.Empty;
        public string UserName { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string ConfirmPassword { get; init; } = string.Empty;
        public string FirstName { get; init; } = string.Empty;
        public string LastName { get; init; } = string.Empty;
        public string? PhoneNumber { get; init; }
    }
}
