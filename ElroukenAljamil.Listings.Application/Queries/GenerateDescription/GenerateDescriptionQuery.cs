using System.Text.Json;
using ElroukenAljamil.BuildingBlocks.Common.Results;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Queries.GenerateDescription;

public record GenerateDescriptionQuery(JsonElement Context) : IRequest<Result<string>>;
