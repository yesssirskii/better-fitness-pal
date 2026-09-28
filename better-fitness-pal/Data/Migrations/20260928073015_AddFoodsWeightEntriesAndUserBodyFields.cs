using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace better_fitness_pal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFoodsWeightEntriesAndUserBodyFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "height_cm",
                table: "users",
                type: "numeric(4,1)",
                precision: 4,
                scale: 1,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "sex",
                table: "users",
                type: "character varying(6)",
                maxLength: 6,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "foods",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    created_by_user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    brand = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    barcode = table.Column<string>(type: "character varying(14)", maxLength: 14, nullable: true),
                    serving_size_g = table.Column<decimal>(type: "numeric(6,1)", precision: 6, scale: 1, nullable: true),
                    calories_per_100g = table.Column<decimal>(type: "numeric(5,1)", precision: 5, scale: 1, nullable: false),
                    protein_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    carbs_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    fat_per_100g = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_foods", x => x.id);
                    table.CheckConstraint("ck_foods_barcode", "barcode ~ '^[0-9]{8,14}$'");
                    table.CheckConstraint("ck_foods_calories", "calories_per_100g BETWEEN 0 AND 900");
                    table.CheckConstraint("ck_foods_carbs", "carbs_per_100g BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_foods_fat", "fat_per_100g BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_foods_protein", "protein_per_100g BETWEEN 0 AND 100");
                    table.CheckConstraint("ck_foods_serving_size_g", "serving_size_g > 0");
                    table.ForeignKey(
                        name: "fk_foods_users_created_by_user_id",
                        column: x => x.created_by_user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "weight_entries",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_date = table.Column<DateOnly>(type: "date", nullable: false),
                    weight_kg = table.Column<decimal>(type: "numeric(5,2)", precision: 5, scale: 2, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weight_entries", x => x.id);
                    table.CheckConstraint("ck_weight_entries_weight_kg", "weight_kg BETWEEN 20 AND 500");
                    table.ForeignKey(
                        name: "fk_weight_entries_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_height_cm",
                table: "users",
                sql: "height_cm BETWEEN 50 AND 300");

            migrationBuilder.AddCheckConstraint(
                name: "ck_users_sex",
                table: "users",
                sql: "sex IN ('male', 'female')");

            migrationBuilder.CreateIndex(
                name: "ix_foods_barcode",
                table: "foods",
                column: "barcode",
                unique: true,
                filter: "deleted_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_foods_created_by_user_id",
                table: "foods",
                column: "created_by_user_id");

            migrationBuilder.CreateIndex(
                name: "ix_weight_entries_user_id_entry_date",
                table: "weight_entries",
                columns: new[] { "user_id", "entry_date" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "foods");

            migrationBuilder.DropTable(
                name: "weight_entries");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_height_cm",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "ck_users_sex",
                table: "users");

            migrationBuilder.DropColumn(
                name: "height_cm",
                table: "users");

            migrationBuilder.DropColumn(
                name: "sex",
                table: "users");
        }
    }
}
