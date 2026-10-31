using ElroukenAljamil.BuildingBlocks.Common.Results;
using ElroukenAljamil.BuildingBlocks.Security.Services;
using ElroukenAljamil.Media.Application.DTOs;
using ElroukenAljamil.Media.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace ElroukenAljamil.Media.Application.Commands.UploadBugAttachment;

public record UploadBugAttachmentCommand : IRequest<Result<UploadResultDto>>
{
    public IFormFile File { get; init; } = null!;
}

public class UploadBugAttachmentCommandHandler : IRequestHandler<UploadBugAttachmentCommand, Result<UploadResultDto>>
{
    private static readonly string[] AllowedExtensions =
    [
        ".jpg", ".jpeg", ".png", ".gif", ".webp", ".avif", ".bmp",
        ".pdf",
        ".doc", ".docx",
        ".xls", ".xlsx",
        ".ppt", ".pptx",
        ".txt", ".csv", ".log"
    ];

    private static readonly Dictionary<string, string> MimeTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [".jpg"]  = "image/jpeg",  [".jpeg"] = "image/jpeg",
        [".png"]  = "image/png",   [".gif"]  = "image/gif",
        [".webp"] = "image/webp",  [".avif"] = "image/avif",
        [".bmp"]  = "image/bmp",
        [".pdf"]  = "application/pdf",
        [".doc"]  = "application/msword",
        [".docx"] = "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        [".xls"]  = "application/vnd.ms-excel",
        [".xlsx"] = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        [".ppt"]  = "application/vnd.ms-powerpoint",
        [".pptx"] = "application/vnd.openxmlformats-officedocument.presentationml.presentation",
        [".txt"]  = "text/plain",  [".csv"]  = "text/csv",
        [".log"]  = "text/plain"
    };

    private const long MaxFileSize = 20 * 1024 * 1024; // 20 MB
    private const string BucketName = "bug-attachments";

    private readonly IFileStorageService _storage;
    private readonly ICurrentUserService _currentUser;

    public UploadBugAttachmentCommandHandler(IFileStorageService storage, ICurrentUserService currentUser)
    {
        _storage     = storage;
        _currentUser = currentUser;
    }

    public async Task<Result<UploadResultDto>> Handle(UploadBugAttachmentCommand request, CancellationToken ct)
    {
        if (_currentUser.UserId == Guid.Empty)
            return Result<UploadResultDto>.Failure("Utilisateur non authentifié.");

        var file = request.File;

        if (file.Length == 0)
            return Result<UploadResultDto>.Failure("Le fichier est vide.");

        if (file.Length > MaxFileSize)
            return Result<UploadResultDto>.Failure("La taille maximale est de 20 MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(ext))
            return Result<UploadResultDto>.Failure($"Type de fichier non autorisé : {ext}");

        var contentType = MimeTypes.TryGetValue(ext, out var mime) ? mime : "application/octet-stream";
        var storagePath = $"{_currentUser.UserId}/{DateTime.UtcNow:yyyy/MM}/{Guid.NewGuid()}{ext}";

        await _storage.EnsureBucketExistsAsync(BucketName, ct);

        using var stream = file.OpenReadStream();
        await _storage.UploadAsync(BucketName, storagePath, stream, contentType, ct);

        var url = $"http://localhost:9000/{BucketName}/{storagePath}";

        return Result<UploadResultDto>.Success(new UploadResultDto
        {
            MediaId          = Guid.NewGuid(),
            OriginalFileName = Path.GetFileName(file.FileName),
            Url              = url,
            Status           = "Uploaded"
        });
    }
}
