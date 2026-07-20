using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace ElroukenAljamil.BuildingBlocks.Security.DevBypass
{
    /// <summary>
    /// Surchargeables via appsettings.Development.json sous la clé "DevUser".
    /// </summary>
    public class DevUserOptions
    {
        public const string SectionName = "DevUser";
        public string UserId { get; set; } = "00000000-0000-0000-0000-000000000001";
        public string Email { get; set; } = "dev@marketplace.local";
        public string UserName { get; set; } = "devuser";
        public string FirstName { get; set; } = "Dev";
        public string LastName { get; set; } = "User";
        public string Role { get; set; } = "Admin";
    }

    /// <summary>
    /// Injecte un ClaimsPrincipal fictif sur chaque requête — satisfait tous les [Authorize] sans token.
    /// N'est enregistré que si ASPNETCORE_ENVIRONMENT == Development.
    /// </summary>
    public class DevAuthBypassMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly DevUserOptions _options;

        public DevAuthBypassMiddleware(RequestDelegate next, IOptions<DevUserOptions> options)
        {
            _next = next;
            _options = options.Value;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, _options.UserId),
                new Claim(ClaimTypes.Email,           _options.Email),
                new Claim(ClaimTypes.Name,            _options.UserName),
                new Claim(ClaimTypes.GivenName,       _options.FirstName),
                new Claim(ClaimTypes.Surname,         _options.LastName),
                new Claim(ClaimTypes.Role,            _options.Role),
                new Claim("full_name", $"{_options.FirstName} {_options.LastName}")
            };

            context.User = new ClaimsPrincipal(new ClaimsIdentity(claims, "DevBypass"));
            await _next(context);
        }
    }
}
