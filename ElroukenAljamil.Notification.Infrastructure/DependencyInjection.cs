using ElroukenAljamil.BuildingBlocks.EventBus.Extensions;
using ElroukenAljamil.Notification.Application.Interfaces;
using ElroukenAljamil.Notification.Domain.Interfaces;
using ElroukenAljamil.Notification.Infrastructure.Hubs;
using ElroukenAljamil.Notification.Infrastructure.Persistence;
using ElroukenAljamil.Notification.Infrastructure.Persistence.Repositories;
using ElroukenAljamil.Notification.Infrastructure.Senders;
using ElroukenAljamil.Notification.Infrastructure.Services;
using ElroukenAljamil.Notification.Infrastructure.Templates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace ElroukenAljamil.Notification.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddNotificationInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // SQL Server via EF Core
            services.AddDbContext<NotificationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("NotificationDb"),
                    b => b.MigrationsAssembly(typeof(NotificationDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITemplateRepository, TemplateRepository>();
            services.AddScoped<IPreferenceRepository, PreferenceRepository>();
            services.AddScoped<IDeliveryMetricRepository, DeliveryMetricRepository>();
            services.AddScoped<IDigestScheduleRepository, DigestScheduleRepository>();

            // Template renderer
            services.AddScoped<ITemplateRenderer, ScribanTemplateRenderer>();

            // Senders
            services.AddScoped<INotificationSender, EmailSender>();
            services.AddScoped<INotificationSender, InAppSender>();
            services.AddScoped<INotificationSender, PushSender>();
            services.AddScoped<INotificationSender, SmsSender>();
            services.AddHttpClient<SmsSender>();

            // Email sender (MailKit)
            services.AddScoped<IEmailSender, MailKitEmailSender>();

            // Recipient resolver (HTTP vers Identity API)
            services.AddScoped<IRecipientResolver, HttpRecipientResolver>();
            services.AddHttpClient<HttpRecipientResolver>();

            // Application services
            services.AddScoped<IDeliveryMetricsService, DeliveryMetricsService>();
            services.AddScoped<IDigestService, DigestService>();
            services.AddScoped<IRealTimeNotificationService, SignalRNotificationService>();

            // Redis pour SignalR backplane + connection tracker
            var redisConn = configuration.GetConnectionString("Redis") ?? "localhost:6379";
            services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));
            services.AddSingleton<IUserConnectionTracker, RedisUserConnectionTracker>();

            // SignalR avec Redis backplane
            services.AddSignalR().AddStackExchangeRedis(redisConn);

            // MassTransit + RabbitMQ
            services.AddEventBus(configuration);

            return services;
        }
    }
}
