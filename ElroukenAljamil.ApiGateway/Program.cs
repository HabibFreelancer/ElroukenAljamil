using ElroukenAljamil.ApiGateway.Middleware;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Ocelot.Cache.CacheManager;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Polly;


var builder = WebApplication.CreateBuilder(args);

// =============================================================
// CONFIGURATION
// =============================================================

// Charger la configuration Ocelot depuis les fichiers JSON
// ocelot.json contient les routes, ocelot.{Environment}.json les surcharges
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

// =============================================================
// SERVICES
// =============================================================

// --- Ocelot (reverse proxy + cache + resilience) ---
builder.Services
    .AddOcelot(builder.Configuration)
    .AddCacheManager(settings => settings.WithDictionaryHandle()) // Cache en mémoire
    .AddPolly(); // Circuit breaker et retry via Polly

// --- Authentification JWT ---
// Le gateway valide les tokens JWT avant de router vers les microservices.
// Cela évite que chaque microservice reçoive des requêtes non authentifiées.
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
        options.RequireHttpsMetadata = false; // true en production
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

// --- CORS ---
// Autoriser les appels depuis le frontend (React, Angular, etc.)
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
            .AllowCredentials(); // Nécessaire pour SignalR
    });
});

// --- Health Checks UI ---
// Dashboard qui agrège les health checks de tous les microservices
builder.Services
    .AddHealthChecksUI(options =>
    {
        options.SetEvaluationTimeInSeconds(30); // Vérifier toutes les 30 secondes
        options.MaximumHistoryEntriesPerEndpoint(50);

        // Enregistrer les endpoints de santé de chaque microservice
        options.AddHealthCheckEndpoint("Listings Service", "http://localhost:5001/health");
        options.AddHealthCheckEndpoint("Identity Service", "http://localhost:5002/health");
        options.AddHealthCheckEndpoint("Messaging Service", "http://localhost:5003/health");
        options.AddHealthCheckEndpoint("Media Service", "http://localhost:5004/health");
        options.AddHealthCheckEndpoint("Search Service", "http://localhost:5005/health");
        options.AddHealthCheckEndpoint("Notification Service", "http://localhost:5006/health");
    })
    .AddInMemoryStorage(); // Stockage en mémoire (pas de BDD supplémentaire)

// --- Swagger agrégé ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// --- Logging ---
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();

// =============================================================
// MIDDLEWARE PIPELINE
// =============================================================

// --- CORS (doit être avant tout le reste) ---
app.UseCors("MarketplaceCors");

// --- Dev : injecte automatiquement un token admin si absent ---
if (app.Environment.IsDevelopment())
    app.UseMiddleware<DevAdminTokenMiddleware>();

// --- Swagger agrégé (disponible uniquement en développement) ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        // Ajouter les Swagger de chaque microservice dans l'UI
        options.SwaggerEndpoint("/listings/swagger/v1/swagger.json", "Listings API");
        options.SwaggerEndpoint("/identity/swagger/v1/swagger.json", "Identity API");
        options.SwaggerEndpoint("/messaging/swagger/v1/swagger.json", "Messaging API");
        options.SwaggerEndpoint("/media/swagger/v1/swagger.json", "Media API");
        options.SwaggerEndpoint("/search/swagger/v1/swagger.json", "Search API");
        options.SwaggerEndpoint("/notifications/swagger/v1/swagger.json", "Notification API");
        options.RoutePrefix = "swagger";
    });
}

// --- Health Checks UI Dashboard ---
// Accessible sur /healthchecks-ui pour voir l'état de tous les services
app.MapHealthChecksUI(options =>
{
    options.UIPath = "/healthchecks-ui";
    options.ApiPath = "/healthchecks-api";
});

// --- Endpoint racine (pour vérifier que le gateway est up) ---
app.MapGet("/", () => Results.Ok(new
{
    service = "Marketplace API Gateway",
    status = "running",
    version = "1.0.0",
    timestamp = DateTime.UtcNow
}));

// --- Ocelot (doit être le dernier middleware) ---
// Ocelot prend le contrôle du routing et redirige vers les microservices
await app.UseOcelot();

app.Run();
