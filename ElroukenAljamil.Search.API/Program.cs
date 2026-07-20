using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using ElroukenAljamil.Search.Application;
using ElroukenAljamil.Search.Infrastructure;
using ElroukenAljamil.Search.Infrastructure.HostedServices;

var builder = WebApplication.CreateBuilder(args);

// --- Couches applicatives ---
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// --- Authentification JWT (BuildingBlocks) ---
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUserService();
builder.Services.AddDevAuthBypass(builder.Configuration);

// --- Initialisation de l'index au d�marrage ---
builder.Services.AddHostedService<IndexInitializationService>();

// --- Response Caching ---
builder.Services.AddResponseCaching();

// --- API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Marketplace Search API", Version = "v1" });
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
    .AddElasticsearch(builder.Configuration["Elasticsearch:Url"] ?? "http://localhost:9200", name: "elasticsearch")
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq");

var app = builder.Build();

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCaching();
app.UseDevAuthBypass(app.Environment);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
