using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Enums;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Identity.Infrastructure.Seed;

public static class AdminSeeder
{
    public static readonly Guid AdminId = new("00000000-0000-0000-0000-000000000001");
    public const string AdminEmail = "admin@elrouken.local";
    public const string AdminPassword = "Admin@1234";

    public static async Task SeedAsync(IdentityDbContext db, IPasswordHasher hasher)
    {
        if (await db.Users.AnyAsync(u => u.Id == AdminId))
            return;

        var admin = ApplicationUser.Create(
            email: AdminEmail,
            userName: "admin",
            passwordHash: hasher.Hash(AdminPassword),
            firstName: "Admin",
            lastName: "ElRouken"
        );

        typeof(ApplicationUser).GetProperty("Id")!.SetValue(admin, AdminId);
        typeof(ApplicationUser).GetProperty("Role")!.SetValue(admin, UserRole.Admin);
        typeof(ApplicationUser).GetProperty("Status")!.SetValue(admin, UserStatus.Active);
        typeof(ApplicationUser).GetProperty("EmailVerifiedAt")!.SetValue(admin, DateTime.UtcNow);

        admin.ClearDomainEvents();

        db.Users.Add(admin);
        await db.SaveChangesAsync();
    }
}
