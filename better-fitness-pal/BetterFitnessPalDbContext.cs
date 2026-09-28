using better_fitness_pal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace better_fitness_pal;

public class BetterFitnessPalDbContext(DbContextOptions<BetterFitnessPalDbContext> options)
    : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>(options)
{
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<WeightEntry> WeightEntries => Set<WeightEntry>();
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>(user =>
        {
            user.ToTable("users", t =>
            {
                t.HasCheckConstraint("ck_users_sex", "sex IN ('male', 'female')");
                t.HasCheckConstraint("ck_users_height_cm", "height_cm BETWEEN 50 AND 300");
            });
            user.Property(u => u.FirstName).HasMaxLength(100);
            user.Property(u => u.LastName).HasMaxLength(100);
            user.Property(u => u.Sex).HasMaxLength(6);
            user.Property(u => u.HeightCm).HasPrecision(4, 1);
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
        
        builder.Entity<Food>(food =>
        {
            food.ToTable("foods", t =>
            {
                t.HasCheckConstraint("ck_foods_barcode", "barcode ~ '^[0-9]{8,14}$'");
                t.HasCheckConstraint("ck_foods_serving_size_g", "serving_size_g > 0");
                t.HasCheckConstraint("ck_foods_calories", "calories_per_100g BETWEEN 0 AND 900");
                t.HasCheckConstraint("ck_foods_protein", "protein_per_100g BETWEEN 0 AND 100");
                t.HasCheckConstraint("ck_foods_carbs", "carbs_per_100g BETWEEN 0 AND 100");
                t.HasCheckConstraint("ck_foods_fat", "fat_per_100g BETWEEN 0 AND 100");
            });

            food.Property(f => f.Name).HasMaxLength(200);
            food.Property(f => f.Brand).HasMaxLength(100);
            food.Property(f => f.Barcode).HasMaxLength(14);
            food.Property(f => f.ServingSizeG).HasPrecision(6, 1);
            food.Property(f => f.CaloriesPer100g).HasPrecision(5, 1).HasColumnName("calories_per_100g");
            food.Property(f => f.ProteinPer100g).HasPrecision(5, 2).HasColumnName("protein_per_100g");
            food.Property(f => f.CarbsPer100g).HasPrecision(5, 2).HasColumnName("carbs_per_100g");
            food.Property(f => f.FatPer100g).HasPrecision(5, 2).HasColumnName("fat_per_100g");

            food.HasIndex(f => f.Barcode).IsUnique().HasFilter("deleted_at IS NULL");

            food.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(f => f.CreatedByUserId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<WeightEntry>(entry =>
        {
            entry.ToTable("weight_entries", t =>
                t.HasCheckConstraint("ck_weight_entries_weight_kg", "weight_kg BETWEEN 20 AND 500"));

            entry.Property(e => e.WeightKg).HasPrecision(5, 2);
            entry.HasIndex(e => new { e.UserId, e.EntryDate }).IsUnique();

            entry.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
    }
}