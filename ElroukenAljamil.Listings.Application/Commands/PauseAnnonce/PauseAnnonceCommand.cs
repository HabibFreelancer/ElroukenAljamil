using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.PauseAnnonce;

public record PauseAnnonceCommand(int AnnonceId) : IRequest<string?>;

public class PauseAnnonceCommandHandler : IRequestHandler<PauseAnnonceCommand, string?>
{
    private readonly IAnnonceRepository _repository;
    public PauseAnnonceCommandHandler(IAnnonceRepository repository) => _repository = repository;

    public Task<string?> Handle(PauseAnnonceCommand request, CancellationToken ct) =>
        _repository.PauseAnnonceAsync(request.AnnonceId, ct);
}
