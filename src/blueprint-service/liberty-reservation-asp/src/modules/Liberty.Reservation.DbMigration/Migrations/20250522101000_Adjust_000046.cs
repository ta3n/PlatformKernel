using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Liberty.Reservation.DbMigration.Migrations
{
    /// <inheritdoc />
    public partial class Adjust_000046 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "booking_reception_end",
                schema: "public",
                table: "plan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "booking_reception_start",
                schema: "public",
                table: "plan",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "use_booking_reception",
                schema: "public",
                table: "plan",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "booking_reception_end",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "booking_reception_start",
                schema: "public",
                table: "plan");

            migrationBuilder.DropColumn(
                name: "use_booking_reception",
                schema: "public",
                table: "plan");
        }
    }
}
