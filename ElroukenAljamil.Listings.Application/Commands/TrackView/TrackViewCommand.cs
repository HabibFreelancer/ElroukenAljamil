using ElroukenAljamil.Listings.Domain.Entities;
using ElroukenAljamil.Listings.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Listings.Application.Commands.TrackView;

public record TrackViewCommand(int AnnonceId, string UserId) : IRequest;

public class TrackViewCommandHandler : IRequestHandler<TrackViewCommand>
{
    private readonly IAnnonceRepository _repository;
    public TrackViewCommandHandler(IAnnonceRepository repository) => _repository = repository;

    public async Task Handle(TrackViewCommand request, CancellationToken ct)
    {
        await _repository.TrackViewAsync(request.AnnonceId, request.UserId, ct);
    }
}
