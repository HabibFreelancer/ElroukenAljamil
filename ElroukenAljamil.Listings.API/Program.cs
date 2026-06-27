using ElroukenAljamil.Listings.Application;
using ElroukenAljamil.Listings.Infrastructure;
using ElroukenAljamil.Listings.API.Middleware;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer; // Ajouté pour AddJwtBearer

var builder = WebApplication.CreateBuilder(args);

// ---- Enregistrement des couches Clean Architecture ----
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

// ---- Configuration API ----
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Marketplace - Listings API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Entrez le token JWT",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

// ---- Authentification JWT ----
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme) // Correction ici
    .AddJwtBearer(options => // Correction ici
    {
        options.Authority = builder.Configuration["Identity:Authority"];
        options.Audience = builder.Configuration["Identity:Audience"];
        options.RequireHttpsMetadata = false;
    });

builder.Services.AddAuthorization();

// ---- Health Checks (Corrigé pour SQL Server) ----
builder.Services.AddHealthChecks()
    .AddSqlServer(builder.Configuration.GetConnectionString("ListingsDb")!);

// ---- CORS ----
builder.Services.AddCors(options =>
{
    options.AddPolicy("Default", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// ---- Pipeline HTTP ----
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors("Default");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
