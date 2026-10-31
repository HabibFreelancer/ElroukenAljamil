using ElroukenAljamil.ApiGateway.Middleware;
using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Web;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;

// Charger NLog avant tout (capture les erreurs de démarrage)
var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";
var nlogConfigFile = File.Exists($"nlog.{env}.config") ? $"nlog.{env}.config" : "nlog.config";
var logger = LogManager.Setup().LoadConfigurationFromFile(nlogConfigFile).GetCurrentClassLogger();

try
{
    logger.Info("=== ApiGateway démarrage — environnement : {0} ===", env);

    var builder = WebApplication.CreateBuilder(args);

    if (!builder.Environment.IsDevelopment())
        builder.Configuration.AddVaultSecrets(env, "ApiGateway");

    // --- NLog ---
    builder.Logging.ClearProviders();
    builder.Host.UseNLog();

    // =============================================================
    // CONFIGURATION
    // =============================================================
    builder.Configuration
        .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
        .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

    // =============================================================
    // SERVICES
    // =============================================================

    builder.Services
        .AddOcelot(builder.Configuration)
        .AddCacheManager(settings => settings.WithDictionaryHandle())
        .AddPolly();

    var jwtSecret = builder.Configuration["Jwt:Secret"]
        ?? throw new InvalidOperationException("Jwt:Secret manquant dans la configuration.");

    builder.Services
        .AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer("Bearer", options =>
        {
            options.RequireHttpsMetadata = !builder.Environment.IsDevelopment();
            options.SaveToken = true;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
                ValidateIssuer = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "Marketplace.Identity",
                ValidateAudience = true,
                ValidAudience = builder.Configuration["Jwt:Audience"] ?? "Marketplace",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        });

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("MarketplaceCors", policy =>
        {
            policy
                .WithOrigins(
                    builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                    ?? new[] { "http://localhost:3000", "http://localhost:4200" })
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
    });

    builder.Services
        .AddHealthChecksUI(options =>
        {
            options.SetEvaluationTimeInSeconds(30);
            options.MaximumHistoryEntriesPerEndpoint(50);
            options.AddHealthCheckEndpoint("Listings Service",     "http://localhost:5001/health");
            options.AddHealthCheckEndpoint("Identity Service",     "http://localhost:5002/health");
            options.AddHealthCheckEndpoint("Messaging Service",    "http://localhost:5003/health");
            options.AddHealthCheckEndpoint("Media Service",        "http://localhost:5004/health");
            options.AddHealthCheckEndpoint("Search Service",       "http://localhost:5005/health");
            options.AddHealthCheckEndpoint("Notification Service", "http://localhost:5006/health");
        })
        .AddInMemoryStorage();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    var app = builder.Build();

    // =============================================================
    // MIDDLEWARE PIPELINE
    // =============================================================

    app.UseCors("MarketplaceCors");

    if (app.Environment.IsDevelopment())
        app.UseMiddleware<DevAdminTokenMiddleware>();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/listings/swagger/v1/swagger.json",      "Listings API");
            options.SwaggerEndpoint("/identity/swagger/v1/swagger.json",      "Identity API");
            options.SwaggerEndpoint("/messaging/swagger/v1/swagger.json",     "Messaging API");
            options.SwaggerEndpoint("/media/swagger/v1/swagger.json",         "Media API");
            options.SwaggerEndpoint("/search/swagger/v1/swagger.json",        "Search API");
            options.SwaggerEndpoint("/notifications/swagger/v1/swagger.json", "Notification API");
            options.RoutePrefix = "swagger";
        });
    }

    app.MapHealthChecksUI(options =>
    {
        options.UIPath  = "/healthchecks-ui";
        options.ApiPath = "/healthchecks-api";
    });

    app.MapGet("/", () => Results.Ok(new
    {
        service   = "Marketplace API Gateway",
        status    = "running",
        version   = "1.0.0",
        timestamp = DateTime.UtcNow
    }));

    await app.UseOcelot();

    app.Run();

    logger.Info("=== ApiGateway arrêté proprement ===");
}
catch (Exception ex)
{
    logger.Fatal(ex, "ApiGateway arrêté suite à une exception non gérée.");
    throw;
}
finally
{
    LogManager.Shutdown();
}
