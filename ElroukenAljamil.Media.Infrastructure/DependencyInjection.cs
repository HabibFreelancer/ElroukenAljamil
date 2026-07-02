using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using ElroukenAljamil.Media.Application.Interfaces;
using ElroukenAljamil.Media.Domain.Interfaces;
using ElroukenAljamil.Media.Infrastructure.Consumers;
using ElroukenAljamil.Media.Infrastructure.Persistence;
using ElroukenAljamil.Media.Infrastructure.Repositories;
using ElroukenAljamil.Media.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;

namespace ElroukenAljamil.Media.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // --- SQLServer + EF Core (métadonnées des fichiers) ---
            services.AddDbContext<MediaDbContext>(options =>
            options.UseSqlServer(
          configuration.GetConnectionString("MediaDb"),
          b => b.MigrationsAssembly(typeof(MediaDbContext).Assembly.FullName)));


            // --- MinIO (Object Storage S3-compatible) ---
            services.AddSingleton<IMinioClient>(sp =>
            {
                var minioConfig = configuration.GetSection("MinIO");
                return new MinioClient()
                    .WithEndpoint(minioConfig["Endpoint"] ?? "localhost:9000")
                    .WithCredentials(
                        minioConfig["AccessKey"] ?? "minioadmin",
                        minioConfig["SecretKey"] ?? "minioadmin")
                    .WithSSL(bool.Parse(minioConfig["UseSSL"] ?? "false"))
                    .Build();
            });

            // --- Repositories ---
            services.AddScoped<IMediaFileRepository, MediaFileRepository>();

            // --- Services ---
            services.AddScoped<IFileStorageService, MinioStorageService>();
            services.AddScoped<IImageProcessingService, ImageSharpProcessingService>();

            // --- EventBus (MassTransit + RabbitMQ) avec consumers ---
            services.AddEventBus(configuration, busConfig =>
            {
                busConfig.AddConsumer<MediaUploadedConsumer>();
                busConfig.AddConsumer<ListingDeletedMediaCleanupConsumer>();
            });

            return services;
        }
    }
}
