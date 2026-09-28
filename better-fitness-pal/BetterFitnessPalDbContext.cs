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
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<UserSettings> UserSettings => Set<UserSettings>();
    public DbSet<SavedMeal> SavedMeals => Set<SavedMeal>();
    public DbSet<SavedMealItem> SavedMealItems => Set<SavedMealItem>();
    public DbSet<DiaryEntry> DiaryEntries => Set<DiaryEntry>();
    
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
        
        builder.Entity<RefreshToken>(token =>
        {
            token.ToTable("refresh_tokens", t =>
                t.HasCheckConstraint("ck_refresh_tokens_expiry", "expires_at > created_at"));

            token.HasIndex(r => r.TokenHash).IsUnique();

            token.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            token.HasOne<RefreshToken>()
                .WithMany()
                .HasForeignKey(r => r.ReplacedByTokenId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        builder.Entity<UserSettings>(settings =>
        {
            settings.ToTable("user_settings", t =>
            {
                t.HasCheckConstraint("ck_user_settings_weight_unit", "weight_unit IN ('kg', 'lb')");
                t.HasCheckConstraint("ck_user_settings_weight_goal_kg", "weight_goal_kg > 0");
                t.HasCheckConstraint("ck_user_settings_weekly_goal_kg", "weekly_goal_kg BETWEEN -1 AND 1");
                t.HasCheckConstraint("ck_user_settings_activity_level",
                    "activity_level IN ('not_very_active', 'lightly_active', 'active', 'very_active')");
                t.HasCheckConstraint("ck_user_settings_calorie_goal", "calorie_goal > 0");
                t.HasCheckConstraint("ck_user_settings_macro_pct_range",
                    "protein_pct BETWEEN 0 AND 100 AND carbs_pct BETWEEN 0 AND 100 AND fat_pct BETWEEN 0 AND 100");
                t.HasCheckConstraint("ck_user_settings_macro_split",
                    "(protein_pct IS NULL AND carbs_pct IS NULL AND fat_pct IS NULL) OR " +
                    "(protein_pct IS NOT NULL AND carbs_pct IS NOT NULL AND fat_pct IS NOT NULL " +
                    "AND protein_pct + carbs_pct + fat_pct = 100)");
            });

            settings.HasKey(s => s.UserId);
            settings.Property(s => s.WeightUnit).HasMaxLength(2);
            settings.Property(s => s.WeightGoalKg).HasPrecision(5, 2);
            settings.Property(s => s.WeeklyGoalKg).HasPrecision(3, 2);
            settings.Property(s => s.ActivityLevel).HasMaxLength(20);

            settings.HasOne<AppUser>()
                .WithOne()
                .HasForeignKey<UserSettings>(s => s.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SavedMeal>(meal =>
        {
            meal.ToTable("saved_meals", t =>
                t.HasCheckConstraint("ck_saved_meals_name", "length(trim(name)) > 0"));

            meal.Property(m => m.Name).HasMaxLength(200);

            meal.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(m => m.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            meal.HasMany(m => m.Items)
                .WithOne()
                .HasForeignKey(i => i.SavedMealId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<SavedMealItem>(item =>
        {
            item.ToTable("saved_meal_items", t =>
                t.HasCheckConstraint("ck_saved_meal_items_grams", "grams > 0"));

            item.Property(i => i.Grams).HasPrecision(6, 1);
            item.HasIndex(i => new { i.SavedMealId, i.FoodId }).IsUnique();

            item.HasOne(i => i.Food)
                .WithMany()
                .HasForeignKey(i => i.FoodId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        builder.Entity<DiaryEntry>(entry =>
        {
            entry.ToTable("diary_entries", t =>
            {
                t.HasCheckConstraint("ck_diary_entries_meal_slot", "meal_slot BETWEEN 1 AND 6");
                t.HasCheckConstraint("ck_diary_entries_grams", "grams > 0");
                t.HasCheckConstraint("ck_diary_entries_calories", "calories_per_100g BETWEEN 0 AND 900");
                t.HasCheckConstraint("ck_diary_entries_protein", "protein_per_100g BETWEEN 0 AND 100");
                t.HasCheckConstraint("ck_diary_entries_carbs", "carbs_per_100g BETWEEN 0 AND 100");
                t.HasCheckConstraint("ck_diary_entries_fat", "fat_per_100g BETWEEN 0 AND 100");
            });

            entry.Property(e => e.Grams).HasPrecision(6, 1);
            entry.Property(e => e.FoodName).HasMaxLength(200);
            entry.Property(e => e.CaloriesPer100g).HasPrecision(5, 1).HasColumnName("calories_per_100g");
            entry.Property(e => e.ProteinPer100g).HasPrecision(5, 2).HasColumnName("protein_per_100g");
            entry.Property(e => e.CarbsPer100g).HasPrecision(5, 2).HasColumnName("carbs_per_100g");
            entry.Property(e => e.FatPer100g).HasPrecision(5, 2).HasColumnName("fat_per_100g");

            entry.HasIndex(e => new { e.UserId, e.EntryDate });

            entry.HasOne<AppUser>()
                .WithMany()
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entry.HasOne<Food>()
                .WithMany()
                .HasForeignKey(e => e.FoodId)
                .OnDelete(DeleteBehavior.SetNull);

            entry.HasOne<SavedMeal>()
                .WithMany()
                .HasForeignKey(e => e.SavedMealId)
                .OnDelete(DeleteBehavior.SetNull);
        });
    
        builder.Entity<IdentityUserRole<Guid>>().ToTable("user_roles");
        builder.Entity<IdentityUserClaim<Guid>>().ToTable("user_claims");
        builder.Entity<IdentityUserLogin<Guid>>().ToTable("user_logins");
        builder.Entity<IdentityUserToken<Guid>>().ToTable("user_tokens");
        builder.Entity<IdentityRoleClaim<Guid>>().ToTable("role_claims");
    }
}