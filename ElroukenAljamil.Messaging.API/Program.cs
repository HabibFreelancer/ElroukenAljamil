using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using ElroukenAljamil.Messaging.Application;
using ElroukenAljamil.Messaging.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Web;

var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var nlogConfigFile = File.Exists($"nlog.{env}.config") ? $"nlog.{env}.config" : "nlog.config";
var logger = LogManager.Setup().LoadConfigurationFromFile(nlogConfigFile).GetCurrentClassLogger();

try
{
    logger.Info("=== Messaging.API démarrage — environnement : {0} ===", env);

    var builder = WebApplication.CreateBuilder(args);

    if (!builder.Environment.IsDevelopment())
        builder.Configuration.AddVaultSecrets(env, "Messaging");

    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    builder.Services.AddApplication();
    builder.Services.AddInfrastructure(builder.Configuration);
    builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment);
    builder.Services.AddMarketplaceAuthorization();
    builder.Services.AddCurrentUserService();
    builder.Services.AddDevAuthBypass(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new() { Title = "Marketplace Messaging API", Version = "v1" });
        options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Entrez le token JWT : Bearer {token}",
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
        });
        options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                    {
                        Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    });

    var rabbitMqCs = builder.Configuration.GetConnectionString("RabbitMQ");
    var healthChecks = builder.Services.AddHealthChecks()
        .AddSqlServer(builder.Configuration.GetConnectionString("MessagingDb")!);
    if (!string.IsNullOrWhiteSpace(rabbitMqCs))
        healthChecks.AddRabbitMQ(rabbitMqCs, name: "rabbitmq");

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<ElroukenAljamil.Messaging.Infrastructure.Persistence.MessagingDbContext>();
        await db.Database.MigrateAsync();
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseDevAuthBypass(app.Environment);
    app.UseAuthentication();
    app.UseAuthorization();
    app.MapControllers();
    app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        ResponseWriter = HealthChecks.UI.Client.UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.Run();

    logger.Info("=== Messaging.API arrêté proprement ===");
}
catch (Exception ex)
{
    logger.Fatal(ex, "Messaging.API arrêté suite à une exception non gérée.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
