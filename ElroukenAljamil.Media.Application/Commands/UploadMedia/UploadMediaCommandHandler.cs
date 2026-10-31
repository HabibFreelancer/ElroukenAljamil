using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.EventBus.Abstractions;
using ElroukenAljamil.BuildingBlocks.EventBus.Events;
using ElroukenAljamil.BuildingBlocks.EventBus.Events.Media;
using ElroukenAljamil.BuildingBlocks.Events.Abstractions;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Media.Application.DTOs;
using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Entities;
using ElroukenAljamil.Media.Domain.Interfaces;
using MediatR;

namespace ElroukenAljamil.Media.Application.Commands.UploadMedia
{
    public class UploadMediaCommandHandler : IRequestHandler<UploadMediaCommand, Result<UploadResultDto>>
    {
        private readonly IMediaFileRepository _repository;
        private readonly IFileStorageService _storageService;
        private readonly IImageProcessingService _imageProcessingService;
        private readonly ICurrentUserService _currentUser;
        private readonly IEventBus _eventBus;

        private const string BucketName = "marketplace-media";

        public UploadMediaCommandHandler(
            IMediaFileRepository repository,
            IFileStorageService storageService,
            IImageProcessingService imageProcessingService,
            ICurrentUserService currentUser,
            IEventBus eventBus)
        {
            _repository = repository;
            _storageService = storageService;
            _imageProcessingService = imageProcessingService;
            _currentUser = currentUser;
            _eventBus = eventBus;
        }

        public async Task<Result<UploadResultDto>> Handle(UploadMediaCommand request, CancellationToken ct)
        {
            if (_currentUser.UserId == Guid.Empty)
                return Result<UploadResultDto>.Failure("Utilisateur non authentifié.");

            var file = request.File;

            // Lire le fichier en mémoire une seule fois
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0;

            // Valider que c'est réellement une image
            var isValid = await _imageProcessingService.IsValidImageAsync(memoryStream, ct);
            if (!isValid)
                return Result<UploadResultDto>.Failure("Le fichier n'est pas une image valide.");

            // Lire les dimensions
            memoryStream.Position = 0;
            var dimensions = await _imageProcessingService.GetDimensionsAsync(memoryStream, ct);

            // Générer un chemin de stockage unique
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            var storagePath = $"{_currentUser.UserId}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{fileExtension}";

            // Upload vers MinIO
            await _storageService.EnsureBucketExistsAsync(BucketName, ct);

            memoryStream.Position = 0;
            await _storageService.UploadAsync(BucketName, storagePath, memoryStream, file.ContentType, ct);

            // Créer l'entité domain
            var mediaFile = MediaFile.Create(
                originalFileName: file.FileName,
                storagePath: storagePath,
                contentType: file.ContentType,
                fileSize: file.Length,
                width: dimensions.Width,
                height: dimensions.Height,
                ownerId: _currentUser.UserId,
                bucketName: BucketName);

            await _repository.AddAsync(mediaFile, ct);

            // Publier l'événement pour déclencher le traitement asynchrone
            await _eventBus.PublishAsync(new MediaUploadedEvent
            {
                MediaId = mediaFile.Id,
                OwnerId = mediaFile.OwnerId,
                StoragePath = mediaFile.StoragePath,
                BucketName = BucketName,
                ContentType = mediaFile.ContentType,
                UploadedAt = DateTime.UtcNow
            }, ct);

            var result = new UploadResultDto
            {
                MediaId = mediaFile.Id,
                OriginalFileName = mediaFile.OriginalFileName,
                Url = mediaFile.GetPublicUrl($"http://localhost:9000"),
                Status = mediaFile.Status.ToString()
            };

            return Result<UploadResultDto>.Success(result);
        }
    }
}
