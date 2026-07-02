using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Media.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Media.Application.Commands.AssignMedia
{
    public class AssignMediaCommandHandler : IRequestHandler<AssignMediaCommand, Result>
    {
        private readonly IMediaFileRepository _repository;
        private readonly ICurrentUserService _currentUser;

        private const int MaxImagesPerListing = 15;

        public AssignMediaCommandHandler(
            IMediaFileRepository repository,
            ICurrentUserService currentUser)
        {
            _repository = repository;
            _currentUser = currentUser;
        }

        public async Task<Result> Handle(AssignMediaCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result.Failure("Utilisateur non authentifié.");

            if (request.MediaIds.Count == 0)
                return Result.Failure("Aucun média à assigner.");

            if (request.MediaIds.Count > MaxImagesPerListing)
                return Result.Failure($"Maximum {MaxImagesPerListing} images par annonce.");

            // Vérifier le nombre total d'images déjà assignées
            var existingCount = await _repository.GetCountByListingAsync(request.ListingId, ct);
            if (existingCount + request.MediaIds.Count > MaxImagesPerListing)
                return Result.Failure($"L'annonce aurait plus de {MaxImagesPerListing} images au total.");

            var sortOrder = existingCount;

            foreach (var mediaId in request.MediaIds)
            {
                var mediaFile = await _repository.GetByIdAsync(mediaId, ct);
                if (mediaFile is null)
                    return Result.Failure($"Média {mediaId} introuvable.");

                if (!mediaFile.IsOwnedBy(_currentUser.UserId))
                    return Result.Failure($"Vous n'êtes pas propriétaire du média {mediaId}.");

                mediaFile.AssignToListing(request.ListingId, sortOrder);
                await _repository.UpdateAsync(mediaFile, ct);
                sortOrder++;
            }

            return Result.Success();
        }
    }

}
