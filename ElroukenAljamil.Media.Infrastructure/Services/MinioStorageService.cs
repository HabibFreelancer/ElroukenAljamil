using ElroukenAljamil.Media.Application.Interfaces;
using Microsoft.Extensions.Logging;
using Minio;
using Minio.DataModel.Args;

namespace ElroukenAljamil.Media.Infrastructure.Services
{
    /// <summary>
    /// Implémentation du stockage de fichiers via MinIO (S3-compatible).
    /// </summary>
    public class MinioStorageService : IFileStorageService
    {
        private readonly IMinioClient _minioClient;
        private readonly ILogger<MinioStorageService> _logger;

        public MinioStorageService(IMinioClient minioClient, ILogger<MinioStorageService> logger)
        {
            _minioClient = minioClient;
            _logger = logger;
        }

        public async Task<string> UploadAsync(
            string bucketName, string objectName, Stream fileStream,
            string contentType, CancellationToken ct = default)
        {
            await EnsureBucketExistsAsync(bucketName, ct);

            var putArgs = new PutObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithStreamData(fileStream)
                .WithObjectSize(fileStream.Length)
                .WithContentType(contentType);

            await _minioClient.PutObjectAsync(putArgs, ct);
            _logger.LogInformation("Fichier uploadé : {Bucket}/{Object}", bucketName, objectName);

            return objectName;
        }

        public async Task<Stream> DownloadAsync(
            string bucketName, string objectName, CancellationToken ct = default)
        {
            var memoryStream = new MemoryStream();

            var getArgs = new GetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithCallbackStream(stream => stream.CopyTo(memoryStream));

            await _minioClient.GetObjectAsync(getArgs, ct);
            memoryStream.Position = 0;

            return memoryStream;
        }

        public async Task DeleteAsync(
            string bucketName, string objectName, CancellationToken ct = default)
        {
            var removeArgs = new RemoveObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName);

            await _minioClient.RemoveObjectAsync(removeArgs, ct);
            _logger.LogInformation("Fichier supprimé : {Bucket}/{Object}", bucketName, objectName);
        }

        public async Task<bool> ExistsAsync(
            string bucketName, string objectName, CancellationToken ct = default)
        {
            try
            {
                var statArgs = new StatObjectArgs()
                    .WithBucket(bucketName)
                    .WithObject(objectName);

                await _minioClient.StatObjectAsync(statArgs, ct);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetPresignedUrlAsync(
            string bucketName, string objectName, TimeSpan expiry, CancellationToken ct = default)
        {
            var presignedArgs = new PresignedGetObjectArgs()
                .WithBucket(bucketName)
                .WithObject(objectName)
                .WithExpiry((int)expiry.TotalSeconds);

            return await _minioClient.PresignedGetObjectAsync(presignedArgs);
        }

        public async Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct = default)
        {
            var existsArgs = new BucketExistsArgs().WithBucket(bucketName);
            var exists = await _minioClient.BucketExistsAsync(existsArgs, ct);

            if (!exists)
            {
                var makeArgs = new MakeBucketArgs().WithBucket(bucketName);
                await _minioClient.MakeBucketAsync(makeArgs, ct);
                _logger.LogInformation("Bucket créé : {Bucket}", bucketName);
            }
        }
    }
}
