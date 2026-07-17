using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.Suggest
{
    public record SuggestQuery : IRequest<Result<SuggestResponseDto>>
    {
        public string Query { get; init; } = string.Empty;
        public int MaxSuggestions { get; init; } = 10;
    }
}
