using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using ElroukenAljamil.BuildingBlocks.Security;
using ElroukenAljamil.Identity.Application;
using ElroukenAljamil.Identity.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


// --- Couches applicatives ---
builder.Services.AddApplication();
builder.Services.AddIdentityInfrastructure(builder.Configuration);

// --- Authentification JWT (BuildingBlocks) ---
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddCurrentUserService();
builder.Services.AddDevAuthBypass(builder.Configuration);

// --- API ---
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Marketplace Identity API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Entrez le token JWT : Bearer {token}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey
    });
});

// --- Health Checks ---
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("IdentityDb")!)
    .AddRabbitMQ(builder.Configuration.GetConnectionString("RabbitMQ")!, name: "rabbitmq");

var app = builder.Build();

// --- Migration automatique ---
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ElroukenAljamil.Identity.Infrastructure.Persistence.IdentityDbContext>();
    await db.Database.MigrateAsync();
}

// --- Middleware Pipeline ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseDevAuthBypass(app.Environment);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();