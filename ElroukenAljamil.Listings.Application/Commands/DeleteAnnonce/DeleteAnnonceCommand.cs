using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.DeleteAnnonce;

public record DeleteAnnonceCommand(int AnnonceId) : IRequest<bool>;

public class DeleteAnnonceCommandHandler : IRequestHandler<DeleteAnnonceCommand, bool>
{
    private readonly IAnnonceRepository _repository;
    public DeleteAnnonceCommandHandler(IAnnonceRepository repository) => _repository = repository;

    public Task<bool> Handle(DeleteAnnonceCommand request, CancellationToken ct) =>
        _repository.DeleteAnnonceAsync(request.AnnonceId, ct);
}
