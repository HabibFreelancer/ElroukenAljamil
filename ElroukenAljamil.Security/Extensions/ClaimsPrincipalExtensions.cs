using System.Security.Claims;

namespace ElroukenAljamil.BuildingBlocks.Security.Extensions
{
    /// <summary>
    /// Extensions pour extraire facilement les informations utilisateur depuis les claims JWT.
    /// </summary>
    public static class ClaimsPrincipalExtensions
    {
        public static Guid GetUserId(this ClaimsPrincipal principal)
        {
            var claim = principal.FindFirst(ClaimTypes.NameIdentifier)
                       ?? principal.FindFirst("sub");

            if (claim is null || !Guid.TryParse(claim.Value, out var userId))
                throw new UnauthorizedAccessException("User ID introuvable dans le token.");

            return userId;
        }

        public static string GetEmail(this ClaimsPrincipal principal)
        {
            return principal.FindFirst(ClaimTypes.Email)?.Value
                   ?? principal.FindFirst("email")?.Value
                   ?? throw new UnauthorizedAccessException("Email introuvable dans le token.");
        }

        public static string GetFullName(this ClaimsPrincipal principal)
        {
            var firstName = principal.FindFirst(ClaimTypes.GivenName)?.Value
                           ?? principal.FindFirst("given_name")?.Value ?? "";
            var lastName = principal.FindFirst(ClaimTypes.Surname)?.Value
                          ?? principal.FindFirst("family_name")?.Value ?? "";

            return $"{firstName} {lastName}".Trim();
        }

        public static IEnumerable<string> GetRoles(this ClaimsPrincipal principal)
        {
            return principal.FindAll(ClaimTypes.Role)
                .Select(c => c.Value);
        }

        public static bool IsAdmin(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("Admin");
        }

        public static bool HasRole(this ClaimsPrincipal principal, string role)
        {
            return principal.IsInRole(role);
        }
    }
}
