using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace ElroukenAljamil.ApiGateway.Middleware;

public class DevAdminTokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly string _token;

    public DevAdminTokenMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _token = GenerateAdminToken(configuration);
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Headers.ContainsKey("Authorization"))
            context.Request.Headers["Authorization"] = $"Bearer {_token}";

        await _next(context);
    }

    private static string GenerateAdminToken(IConfiguration configuration)
    {
        var secret = configuration["Jwt:Secret"]!;
        var issuer = configuration["Jwt:Issuer"]!;
        var audience = configuration["Jwt:Audience"]!;

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "00000000-0000-0000-0000-000000000001"),
            new Claim(ClaimTypes.Email,          "admin@elrouken.local"),
            new Claim(ClaimTypes.Name,           "admin"),
            new Claim(ClaimTypes.GivenName,      "Admin"),
            new Claim(ClaimTypes.Surname,        "ElRouken"),
            new Claim(ClaimTypes.Role,           "Admin"),
            new Claim("full_name",               "Admin ElRouken")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(365),
            signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
