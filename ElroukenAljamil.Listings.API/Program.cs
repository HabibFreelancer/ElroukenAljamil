using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using ElroukenAljamil.Listings.Application;
using ElroukenAljamil.Listings.Infrastructure;
using ElroukenAljamil.Listings.Infrastructure.BackgroundJobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// --- Couches applicatives ---
builder.Services.AddApplication();
builder.Services.AddListingsInfrastructure(builder.Configuration);

// --- Authentification JWT (BuildingBlocks) ---
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUserService();

// --- Background Worker ---
builder.Services.AddHostedService<ListingExpirationWorker>();

// --- API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Marketplace Listings API", Version = "v1" });
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

// --- Health Checks ---
builder.Services.AddHealthChecks()
     .AddSqlServer(builder.Configuration.GetConnectionString("ListingsDb")!)
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq");

var app = builder.Build();

// --- Migration automatique ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ElroukenAljamil.Listings.Infrastructure.Persistence.ListingsDbContext>();
    await db.Database.MigrateAsync();
}

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
