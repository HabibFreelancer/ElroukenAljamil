using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.ToggleFavorite;

public record ToggleFavoriteCommand(int AnnonceId, string UserId) : IRequest<bool>;

public class ToggleFavoriteCommandHandler : IRequestHandler<ToggleFavoriteCommand, bool>
{
    private readonly IAnnonceRepository _repository;
    public ToggleFavoriteCommandHandler(IAnnonceRepository repository) => _repository = repository;

    public Task<bool> Handle(ToggleFavoriteCommand request, CancellationToken ct) =>
        _repository.ToggleFavoriteAsync(request.AnnonceId, request.UserId, ct);
}
