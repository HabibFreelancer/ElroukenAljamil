using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.Listings.Application.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GenerateDescription;

public class GenerateDescriptionQueryHandler : IRequestHandler<GenerateDescriptionQuery, Result<string>>
{
    private readonly IAiDescriptionService _aiService;

    public GenerateDescriptionQueryHandler(IAiDescriptionService aiService)
    {
        _aiService = aiService;
    }

    public async Task<Result<string>> Handle(GenerateDescriptionQuery request, CancellationToken ct)
    {
        var description = await _aiService.GenerateAsync(request.Context, ct);
        return Result<string>.Success(description);
    }
}
