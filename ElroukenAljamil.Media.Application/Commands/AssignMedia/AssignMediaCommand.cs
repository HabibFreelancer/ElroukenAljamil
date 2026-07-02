using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Media.Application.Commands.AssignMedia
{
    public record AssignMediaCommand : IRequest<Result>
    {
        public Guid ListingId { get; init; }
        public List<Guid> MediaIds { get; init; } = new();
    }
}
