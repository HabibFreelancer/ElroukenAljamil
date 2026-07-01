using System.Security.Claims;
using ElroukenAljamil.BuildingBlocks.Security.Extensions;
using Microsoft.AspNetCore.Http;

namespace ElroukenAljamil.BuildingBlocks.Security.Services
{
    /// <summary>
    /// Implémentation qui lit les claims depuis le HttpContext.
    /// </summary>
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;

        public Guid UserId => IsAuthenticated
            ? User!.GetUserId()
            : Guid.Empty;

        public string Email => IsAuthenticated
            ? User!.GetEmail()
            : string.Empty;

        public string FullName => IsAuthenticated
            ? User!.GetFullName()
            : string.Empty;

        public IEnumerable<string> Roles => IsAuthenticated
            ? User!.GetRoles()
            : Enumerable.Empty<string>();

        public bool IsAdmin => IsAuthenticated && User!.IsAdmin();

       
    }
}
