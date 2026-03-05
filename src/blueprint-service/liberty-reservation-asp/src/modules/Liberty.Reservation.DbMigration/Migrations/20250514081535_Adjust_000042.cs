using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000042 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "minimum_price",
                schema: "public",
                table: "plan",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "facility",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "minimum_price",
                schema: "public",
                table: "facility",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "minimum_price",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "is_enabled_minimum_price",
                schema: "public",
                table: "facility");

            migrationBuilder.DropColumn(
                name: "minimum_price",
                schema: "public",
                table: "facility");
        }
    }
}
