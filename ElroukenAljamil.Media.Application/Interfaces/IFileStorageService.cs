using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElroukenAljamil.Media.Application.Interfaces
{
    /// <summary>
    /// Abstraction pour le stockage de fichiers (MinIO/S3).
    /// </summary>
    public interface IFileStorageService
    {
        /// <summary>
        /// Upload un fichier vers le storage.
        /// </summary>
        Task<string> UploadAsync(
            string bucketName,
            string objectName,
            Stream fileStream,
            string contentType,
            CancellationToken ct = default);

        /// <summary>
        /// Télécharge un fichier depuis le storage.
        /// </summary>
        Task<Stream> DownloadAsync(
            string bucketName,
            string objectName,
            CancellationToken ct = default);

        /// <summary>
        /// Supprime un fichier du storage.
        /// </summary>
        Task DeleteAsync(
            string bucketName,
            string objectName,
            CancellationToken ct = default);

        /// <summary>
        /// Vérifie si un fichier existe.
        /// </summary>
        Task<bool> ExistsAsync(
            string bucketName,
            string objectName,
            CancellationToken ct = default);

        /// <summary>
        /// Génère une URL pré-signée pour accès temporaire.
        /// </summary>
        Task<string> GetPresignedUrlAsync(
            string bucketName,
            string objectName,
            TimeSpan expiry,
            CancellationToken ct = default);

        /// <summary>
        /// Assure que le bucket existe, le crée sinon.
        /// </summary>
        Task EnsureBucketExistsAsync(string bucketName, CancellationToken ct = default);
    }
}
