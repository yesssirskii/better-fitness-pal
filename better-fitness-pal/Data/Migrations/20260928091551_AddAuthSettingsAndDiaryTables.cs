using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace better_fitness_pal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAuthSettingsAndDiaryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "refresh_tokens",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "text", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    replaced_by_token_id = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_tokens", x => x.id);
                    table.CheckConstraint("ck_refresh_tokens_expiry", "expires_at > created_at");
                    table.ForeignKey(
                        name: "fk_refresh_tokens_refresh_tokens_replaced_by_token_id",
                        column: x => x.replaced_by_token_id,
                        principalTable: "refresh_tokens",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_refresh_tokens_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_meals",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_meals", x => x.id);
                    table.CheckConstraint("ck_saved_meals_name", "length(trim(name)) > 0");
                    table.ForeignKey(
                        name: "fk_saved_meals_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_settings",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    weight_unit = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    weight_goal_kg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: true),
                    weekly_goal_kg = table.Column<decimal>(type: "numeric(3,2)", precision: 3, scale: 2, nullable: true),
                    activity_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    calorie_goal = table.Column<int>(type: "integer", nullable: true),
                    protein_pct = table.Column<short>(type: "smallint", nullable: true),
                    carbs_pct = table.Column<short>(type: "smallint", nullable: true),
                    fat_pct = table.Column<short>(type: "smallint", nullable: true),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_settings", x => x.user_id);
                    table.CheckConstraint("ck_user_settings_activity_level", "activity_level IN ('not_very_active', 'lightly_active', 'active', 'very_active')");
                    table.CheckConstraint("ck_user_settings_calorie_goal", "calorie_goal > 0");
                    table.CheckConstraint("ck_user_settings_macro_pct_range", "protein_pct BETWEEN 0 AND 100 AND carbs_pct BETWEEN 0 AND 100 AND fat_pct BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_user_settings_macro_split", "(protein_pct IS NULL AND carbs_pct IS NULL AND fat_pct IS NULL) OR (protein_pct IS NOT NULL AND carbs_pct IS NOT NULL AND fat_pct IS NOT NULL AND protein_pct + carbs_pct + fat_pct = 100)");
                    table.CheckConstraint("ck_user_settings_weekly_goal_kg", "weekly_goal_kg BETWEEN -1 AND 1");
                    table.CheckConstraint("ck_user_settings_weight_goal_kg", "weight_goal_kg > 0");
                    table.CheckConstraint("ck_user_settings_weight_unit", "weight_unit IN ('kg', 'lb')");
                    table.ForeignKey(
                        name: "fk_user_settings_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "diary_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    meal_slot = table.Column<short>(type: "smallint", nullable: false),
                    food_id = table.Column<long>(type: "bigint", nullable: true),
                    saved_meal_id = table.Column<long>(type: "bigint", nullable: true),
                    grams = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: false),
                    food_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    calories_per_100g = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    protein_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    carbs_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    fat_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_diary_entries", x => x.id);
                    table.CheckConstraint("ck_diary_entries_calories", "calories_per_100g BETWEEN 0 AND 900");
                    table.CheckConstraint("ck_diary_entries_carbs", "carbs_per_100g BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_diary_entries_fat", "fat_per_100g BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_diary_entries_grams", "grams > 0");
                    table.CheckConstraint("ck_diary_entries_meal_slot", "meal_slot BETWEEN 1 AND 6");
                    table.CheckConstraint("ck_diary_entries_protein", "protein_per_100g BETWEEN 0 AND 100");
                    table.ForeignKey(
                        name: "fk_diary_entries_foods_food_id",
                        column: x => x.food_id,
                        principalTable: "foods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_diary_entries_saved_meals_saved_meal_id",
                        column: x => x.saved_meal_id,
                        principalTable: "saved_meals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_diary_entries_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "saved_meal_items",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    saved_meal_id = table.Column<long>(type: "bigint", nullable: false),
                    food_id = table.Column<long>(type: "bigint", nullable: false),
                    grams = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_saved_meal_items", x => x.id);
                    table.CheckConstraint("ck_saved_meal_items_grams", "grams > 0");
                    table.ForeignKey(
                        name: "fk_saved_meal_items_foods_food_id",
                        column: x => x.food_id,
                        principalTable: "foods",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_saved_meal_items_saved_meals_saved_meal_id",
                        column: x => x.saved_meal_id,
                        principalTable: "saved_meals",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_diary_entries_food_id",
                table: "diary_entries",
                column: "food_id");

            migrationBuilder.CreateIndex(
                name: "ix_diary_entries_saved_meal_id",
                table: "diary_entries",
                column: "saved_meal_id");

            migrationBuilder.CreateIndex(
                name: "ix_diary_entries_user_id_entry_date",
                table: "diary_entries",
                columns: new[] { "user_id", "entry_date" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_replaced_by_token_id",
                table: "refresh_tokens",
                column: "replaced_by_token_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_token_hash",
                table: "refresh_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_refresh_tokens_user_id",
                table: "refresh_tokens",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_meal_items_food_id",
                table: "saved_meal_items",
                column: "food_id");

            migrationBuilder.CreateIndex(
                name: "ix_saved_meal_items_saved_meal_id_food_id",
                table: "saved_meal_items",
                columns: new[] { "saved_meal_id", "food_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_saved_meals_user_id",
                table: "saved_meals",
                column: "user_id");
            
            migrationBuilder.Sql(
                "CREATE UNIQUE INDEX saved_meals_user_name_unique ON saved_meals (user_id, lower(name));");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "diary_entries");

            migrationBuilder.DropTable(
                name: "refresh_tokens");

            migrationBuilder.DropTable(
                name: "saved_meal_items");

            migrationBuilder.DropTable(
                name: "user_settings");

            migrationBuilder.DropTable(
                name: "saved_meals");
        }
    }
}
