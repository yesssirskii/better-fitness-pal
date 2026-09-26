using better_fitness_pal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace better_fitness_pal;

public class BetterFitnessPalDbContext(DbContextOptions<BetterFitnessPalDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(user =>
        {
            user.ToTable("users");
            user.Property(u => u.FirstName).HasMaxLength(100);
            user.Property(u => u.LastName).HasMaxLength(100);
            user.HasIndex(u => u.NormalizedEmail).IsUnique();
        });

        builder.Entity<IdentityRole<Guid>>(role =>
        {
            role.ToTable("roles");
            role.HasData(
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("0199a3c2-6b1e-7a4d-9f2e-3c5b8d7e1a01"),
                    Name = AppUserRoles.Admin,
                    NormalizedName = "ADMIN",
                    ConcurrencyStamp = "0199a3c2-6b1e-7a4d-9f2e-3c5b8d7e1a01"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse("0199a3c2-6b1e-7a4d-9f2e-3c5b8d7e1a02"),
                    Name = AppUserRoles.User,
                    NormalizedName = "USER",
                    ConcurrencyStamp = "0199a3c2-6b1e-7a4d-9f2e-3c5b8d7e1a02"
                });
        });

        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
    }
}