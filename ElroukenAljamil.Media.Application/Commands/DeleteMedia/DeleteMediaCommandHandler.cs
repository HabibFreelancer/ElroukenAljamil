using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Media;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Media.Application.Commands.DeleteMedia
{
    public class DeleteMediaCommandHandler : IRequestHandler<DeleteMediaCommand, Result>
    {
        private readonly IMediaFileRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        public DeleteMediaCommandHandler(
            IMediaFileRepository repository,
            Interfaces.IFileStorageService storageService,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _storageService = storageService;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result> Handle(DeleteMediaCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result.Failure("Utilisateur non authentifié.");

            var mediaFile = await _repository.GetByIdAsync(request.MediaId, ct);
            if (mediaFile is null)
                return Result.Failure($"Média {request.MediaId} introuvable.");

            if (!mediaFile.IsOwnedBy(_currentUser.UserId))
                return Result.Failure("Vous n'êtes pas autorisé à supprimer ce média.");

            // Supprimer le fichier original de MinIO
            await _storageService.DeleteAsync(mediaFile.BucketName, mediaFile.StoragePath, ct);

            // Supprimer les variantes si elles existent
            if (mediaFile.Variants.ThumbnailPath is not null)
                await _storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.ThumbnailPath, ct);
            if (mediaFile.Variants.MediumPath is not null)
                await _storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.MediumPath, ct);
            if (mediaFile.Variants.LargePath is not null)
                await _storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.LargePath, ct);
            if (mediaFile.Variants.WebPPath is not null)
                await _storageService.DeleteAsync(mediaFile.BucketName, mediaFile.Variants.WebPPath, ct);

            // Marquer pour suppression et persister
            mediaFile.MarkForDeletion();
            await _repository.DeleteAsync(mediaFile, ct);

            // Publier l'événement
            await _eventBus.PublishAsync(new MediaDeletedEvent
            {
                MediaId = mediaFile.Id,
                ListingId = mediaFile.ListingId,
                OwnerId = mediaFile.OwnerId,
                DeletedAt = DateTime.UtcNow
            }, ct);

            return Result.Success();
        }
    }
}
