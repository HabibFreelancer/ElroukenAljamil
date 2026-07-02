using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Media.Application.Commands.DeleteMedia
{
    public record DeleteMediaCommand(Guid MediaId) : IRequest<Result>;
}
