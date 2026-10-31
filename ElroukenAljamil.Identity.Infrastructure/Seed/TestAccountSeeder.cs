using ElroukenAljamil.Identity.Application.Interfaces;
using ElroukenAljamil.Identity.Domain.Entities;
using ElroukenAljamil.Identity.Domain.Enums;
using ElroukenAljamil.Identity.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ElroukenAljamil.Identity.Infrastructure.Seed;

public static class TestAccountSeeder
{
    private static readonly (Guid Id, string Email, string UserName, string FirstName, string LastName, string Password, UserRole Role)[] Accounts =
    [
        (
            new Guid("A1000000-0000-0000-0000-000000000001"),
            "recette.admin@elrouken.local",
            "recette.admin",
            "Admin",
            "Recette",
            "Recette@Admin1234",
            UserRole.Admin
        ),
        (
            new Guid("B2000000-0000-0000-0000-000000000001"),
            "testeur1@elrouken.local",
            "testeur1",
            "Testeur",
            "Un",
            "Testeur@1234",
            UserRole.Tester
        ),
        (
            new Guid("B2000000-0000-0000-0000-000000000002"),
            "testeur2@elrouken.local",
            "testeur2",
            "Testeur",
            "Deux",
            "Testeur@1234",
            UserRole.Tester
        ),
        (
            new Guid("B2000000-0000-0000-0000-000000000003"),
            "testeur3@elrouken.local",
            "testeur3",
            "Testeur",
            "Trois",
            "Testeur@1234",
            UserRole.Tester
        ),
    ];

    public static async Task SeedAsync(IdentityDbContext db, IPasswordHasher hasher)
    {
        foreach (var (id, email, userName, firstName, lastName, password, role) in Accounts)
        {
            if (await db.Users.AnyAsync(u => u.Id == id || u.Email == email))
                continue;

            var user = ApplicationUser.Create(
                email: email,
                userName: userName,
                passwordHash: hasher.Hash(password),
                firstName: firstName,
                lastName: lastName
            );

            typeof(ApplicationUser).GetProperty("Id")!.SetValue(user, id);
            typeof(ApplicationUser).GetProperty("Role")!.SetValue(user, role);
            typeof(ApplicationUser).GetProperty("Status")!.SetValue(user, UserStatus.Active);
            typeof(ApplicationUser).GetProperty("EmailVerifiedAt")!.SetValue(user, DateTime.UtcNow);

            user.ClearDomainEvents();
            db.Users.Add(user);
        }

        await db.SaveChangesAsync();
    }
}
