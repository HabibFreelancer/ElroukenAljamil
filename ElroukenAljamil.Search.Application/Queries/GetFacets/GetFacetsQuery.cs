using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.GetFacets
{
    public record GetFacetsQuery : IRequest<Result<FacetsResponseDto>>
    {
        public string? Query { get; init; }
        public string? Category { get; init; }
    }
}
