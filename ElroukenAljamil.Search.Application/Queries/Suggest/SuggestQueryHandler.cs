using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Search.Application.DTOs;
using ElroukenAljamil.Search.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Search.Application.Queries.Suggest
{
    public class SuggestQueryHandler : IRequestHandler<SuggestQuery, Result<SuggestResponseDto>>
    {
        private readonly ISearchQueryService _searchQueryService;

        public SuggestQueryHandler(ISearchQueryService searchQueryService)
        {
            _searchQueryService = searchQueryService;
        }

        public async Task<Result<SuggestResponseDto>> Handle(SuggestQuery request, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(request.Query) || request.Query.Length < 2)
                return Result<SuggestResponseDto>.Success(new SuggestResponseDto());

            var suggestions = await _searchQueryService.SuggestAsync(
                request.Query, request.MaxSuggestions, ct);

            return Result<SuggestResponseDto>.Success(new SuggestResponseDto
            {
                Suggestions = suggestions
            });
        }
    }
}
